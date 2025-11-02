// Comment Mutation Score on Pull Request
// This script posts a comment on the PR with the mutation testing results

const fs = require('fs');
const path = require('path');

async function commentMutationScore({ github, context }) {
    try {
        const strykerOutputDir = 'tests/Connector.UnitTests/StrykerOutput';

        if (!fs.existsSync(strykerOutputDir)) {
            console.log('⚠️ StrykerOutput directory not found');
            return;
        }

        const dirs = fs.readdirSync(strykerOutputDir)
            .filter(f => fs.statSync(path.join(strykerOutputDir, f)).isDirectory())
            .sort()
            .reverse();

        if (dirs.length === 0) {
            console.log('⚠️ No report directories found');
            return;
        }

        const latestReport = path.join(strykerOutputDir, dirs[0], 'reports', 'mutation-report.json');

        if (!fs.existsSync(latestReport)) {
            console.log('⚠️ Mutation report file not found');
            return;
        }

        const report = JSON.parse(fs.readFileSync(latestReport, 'utf8'));

        // Calculate mutation score by counting killed vs total mutants
        let totalMutants = 0;
        let killedMutants = 0;
        let survivedMutants = 0;
        let timeoutMutants = 0;
        let noCoverageMutants = 0;

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
                                    killedMutants++; // Timeouts count as killed
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

        const mutationScore = totalMutants > 0
            ? Math.round((killedMutants / totalMutants) * 10000) / 100
            : 0;

        const threshold = 80;
        const status = mutationScore >= threshold ? '✅' : '❌';
        const qualityGate = mutationScore >= threshold ? 'PASSED' : 'FAILED';
        const emoji = mutationScore >= threshold ? '🎉' : '⚠️';

        const body = `## ${status} Mutation Testing Report

**Mutation Score:** \`${mutationScore.toFixed(2)}%\`  
**Quality Gate:** **${qualityGate}** (Threshold: ${threshold}%)

### 📊 Mutation Statistics

| Metric | Count | Percentage |
|--------|-------|------------|
| 🎯 **Total Mutants** | ${totalMutants} | 100% |
| ✅ **Killed** | ${killedMutants} | ${totalMutants > 0 ? ((killedMutants / totalMutants) * 100).toFixed(1) : 0}% |
| ❌ **Survived** | ${survivedMutants} | ${totalMutants > 0 ? ((survivedMutants / totalMutants) * 100).toFixed(1) : 0}% |
| ⏱️ **Timeout** | ${timeoutMutants} | ${totalMutants > 0 ? ((timeoutMutants / totalMutants) * 100).toFixed(1) : 0}% |
| 🚫 **No Coverage** | ${noCoverageMutants} | ${totalMutants > 0 ? ((noCoverageMutants / totalMutants) * 100).toFixed(1) : 0}% |

---

${mutationScore < threshold
                ? `${emoji} **Action Required:** Mutation score is below ${threshold}%. Please improve test coverage before merging.

**Recommendations:**
- Review the survived mutants in the HTML report
- Add tests to cover the untested code paths
- Ensure assertions properly validate behavior changes`
                : `${emoji} **Excellent work!** Your mutation score of ${mutationScore.toFixed(2)}% exceeds the required threshold.

**Quality Achievements:**
- Strong test coverage that catches code mutations
- Tests effectively validate behavior
- Code changes are well-protected`}

---

📄 [View Full HTML Report](https://github.com/${context.repo.owner}/${context.repo.repo}/actions/runs/${context.runId})
`;

        await github.rest.issues.createComment({
            issue_number: context.issue.number,
            owner: context.repo.owner,
            repo: context.repo.repo,
            body: body
        });

        console.log('✅ Posted mutation score comment to PR');
    } catch (error) {
        console.log('❌ Could not post mutation score comment:', error.message);
        // Don't fail the workflow if comment fails
    }
}

module.exports = commentMutationScore;
