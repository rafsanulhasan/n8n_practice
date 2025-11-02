// Combined Coverage and Mutation Testing Report
// This script posts a single comment with both code coverage and mutation testing results

const fs = require('fs');
const path = require('path');

async function commentCombinedReport({ github, context }) {
    try {
        // ========== CODE COVERAGE ==========
        console.log('Collecting code coverage data...');

        const coverageDir = './coverage';
        let coverageFiles = [];
        let totalLinesCovered = 0;
        let totalLinesValid = 0;
        let totalBranchesCovered = 0;
        let totalBranchesValid = 0;

        function findCoverageFiles(dir) {
            try {
                const files = fs.readdirSync(dir);
                for (const file of files) {
                    const filePath = path.join(dir, file);
                    const stat = fs.statSync(filePath);

                    if (stat.isDirectory()) {
                        findCoverageFiles(filePath);
                    } else if (file === 'coverage.opencover.xml') {
                        coverageFiles.push(filePath);
                    }
                }
            } catch (error) {
                console.log(`Error reading directory ${dir}:`, error.message);
            }
        }

        findCoverageFiles(coverageDir);

        // Parse coverage reports
        if (coverageFiles.length > 0) {
            const { parseStringPromise } = require('xml2js');

            for (const file of coverageFiles) {
                try {
                    const xmlContent = fs.readFileSync(file, 'utf8');
                    const result = await parseStringPromise(xmlContent);

                    if (result.CoverageSession && result.CoverageSession.Summary) {
                        const summary = result.CoverageSession.Summary[0].$;
                        totalLinesCovered += parseInt(summary.visitedSequencePoints || 0);
                        totalLinesValid += parseInt(summary.numSequencePoints || 0);
                        totalBranchesCovered += parseInt(summary.visitedBranchPoints || 0);
                        totalBranchesValid += parseInt(summary.numBranchPoints || 0);
                    }
                } catch (error) {
                    console.log(`Error parsing ${file}:`, error.message);
                }
            }
        }

        const lineCoverage = totalLinesValid > 0
            ? ((totalLinesCovered / totalLinesValid) * 100).toFixed(2)
            : 0;
        const branchCoverage = totalBranchesValid > 0
            ? ((totalBranchesCovered / totalBranchesValid) * 100).toFixed(2)
            : 'N/A';

        const coverageThreshold = 80;
        const coveragePassed = lineCoverage >= coverageThreshold;
        const coverageStatus = coveragePassed ? '✅ PASSED' : '❌ FAILED';

        // ========== MUTATION TESTING ==========
        console.log('Collecting mutation testing data...');

        const strykerOutputDir = 'tests/Connector.UnitTests/StrykerOutput';
        let mutationScore = 0;
        let totalMutants = 0;
        let killedMutants = 0;
        let survivedMutants = 0;
        let timeoutMutants = 0;
        let noCoverageMutants = 0;
        let mutationDataAvailable = false;

        if (fs.existsSync(strykerOutputDir)) {
            const dirs = fs.readdirSync(strykerOutputDir)
                .filter(f => fs.statSync(path.join(strykerOutputDir, f)).isDirectory())
                .sort()
                .reverse();

            if (dirs.length > 0) {
                const latestReport = path.join(strykerOutputDir, dirs[0], 'reports', 'mutation-report.json');

                if (fs.existsSync(latestReport)) {
                    const report = JSON.parse(fs.readFileSync(latestReport, 'utf8'));

                    if (report.files) {
                        Object.values(report.files).forEach(file => {
                            if (file.mutants) {
                                file.mutants.forEach(mutant => {
                                    if (mutant.status !== 'Ignored') {
                                        totalMutants++;
                                        switch (mutant.status) {
                                            case 'Killed':
                                                killedMutants++;
                                                break;
                                            case 'Survived':
                                                survivedMutants++;
                                                break;
                                            case 'Timeout':
                                                timeoutMutants++;
                                                killedMutants++;
                                                break;
                                            case 'NoCoverage':
                                                noCoverageMutants++;
                                                break;
                                        }
                                    }
                                });
                            }
                        });
                    }

                    mutationScore = totalMutants > 0
                        ? Math.round((killedMutants / totalMutants) * 10000) / 100
                        : 0;
                    mutationDataAvailable = true;
                }
            }
        }

        const mutationThreshold = 80;
        const mutationPassed = mutationScore >= mutationThreshold;
        const mutationStatus = mutationPassed ? '✅ PASSED' : '❌ FAILED';

        // ========== OVERALL STATUS ==========
        const allPassed = coveragePassed && mutationPassed;
        const overallEmoji = allPassed ? '🎉' : '⚠️';
        const overallStatus = allPassed ? '✅ ALL CHECKS PASSED' : '❌ SOME CHECKS FAILED';

        // ========== BUILD COMMENT ==========
        const comment = `## ${overallEmoji} Quality Gate Report

### ${overallStatus}

---

### 📊 Code Coverage

| Metric | Value | Status |
|--------|-------|--------|
| **Line Coverage** | **${lineCoverage}%** | ${coverageStatus} |
| Branch Coverage | ${branchCoverage}% | - |
| Lines Covered | ${totalLinesCovered} / ${totalLinesValid} | - |
| Coverage Threshold | ${coverageThreshold}% | - |

${coveragePassed
                ? '✅ **Code coverage threshold met!**'
                : `❌ **Code coverage below threshold!** Current: ${lineCoverage}%, Required: ${coverageThreshold}%`
            }

---

### 🧬 Mutation Testing

${mutationDataAvailable ? `
| Metric | Count | Percentage |
|--------|-------|------------|
| **Mutation Score** | **${mutationScore.toFixed(2)}%** | ${mutationStatus} |
| 🎯 Total Mutants | ${totalMutants} | 100% |
| ✅ Killed | ${killedMutants} | ${totalMutants > 0 ? ((killedMutants / totalMutants) * 100).toFixed(1) : 0}% |
| ❌ Survived | ${survivedMutants} | ${totalMutants > 0 ? ((survivedMutants / totalMutants) * 100).toFixed(1) : 0}% |
| ⏱️ Timeout | ${timeoutMutants} | ${totalMutants > 0 ? ((timeoutMutants / totalMutants) * 100).toFixed(1) : 0}% |
| 🚫 No Coverage | ${noCoverageMutants} | ${totalMutants > 0 ? ((noCoverageMutants / totalMutants) * 100).toFixed(1) : 0}% |

${mutationPassed
                    ? '✅ **Mutation score threshold met!**'
                    : `❌ **Mutation score below threshold!** Current: ${mutationScore.toFixed(2)}%, Required: ${mutationThreshold}%`
                }
` : '⏳ **Mutation testing data not yet available**'}

---

### 📋 Summary

${allPassed
                ? `${overallEmoji} **Excellent work!** Both code coverage and mutation testing thresholds are met.

**Quality Achievements:**
- Strong test coverage (${lineCoverage}%)
- Tests effectively catch mutations (${mutationScore.toFixed(2)}%)
- Code is well-protected against regressions`
                : `${overallEmoji} **Action Required:** Some quality thresholds are not met.

**Recommendations:**
${!coveragePassed ? `- 📈 Increase code coverage to at least ${coverageThreshold}%\n` : ''}${!mutationPassed && mutationDataAvailable ? `- 🧬 Improve mutation score to at least ${mutationThreshold}%\n` : ''}- Review uncovered code and add comprehensive tests
- Ensure tests validate behavior, not just coverage
- Check the detailed reports for specific areas needing attention`
            }

---

📄 [View Full Reports](https://github.com/${context.repo.owner}/${context.repo.repo}/actions/runs/${context.runId})
`;

        // ========== POST COMMENT ==========
        const { data: comments } = await github.rest.issues.listComments({
            owner: context.repo.owner,
            repo: context.repo.repo,
            issue_number: context.issue.number,
        });

        const botComment = comments.find(comment =>
            comment.user.type === 'Bot' && comment.body.includes('Quality Gate Report')
        );

        if (botComment) {
            await github.rest.issues.updateComment({
                owner: context.repo.owner,
                repo: context.repo.repo,
                comment_id: botComment.id,
                body: comment,
            });
            console.log('✅ Updated existing quality gate comment');
        } else {
            await github.rest.issues.createComment({
                owner: context.repo.owner,
                repo: context.repo.repo,
                issue_number: context.issue.number,
                body: comment,
            });
            console.log('✅ Created new quality gate comment');
        }

    } catch (error) {
        console.log('❌ Could not post quality gate comment:', error.message);
        console.error(error);
        // Don't fail the workflow if comment fails
    }
}

module.exports = commentCombinedReport;
