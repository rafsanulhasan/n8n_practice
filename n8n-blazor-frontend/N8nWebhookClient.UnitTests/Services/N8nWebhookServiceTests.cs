using System.Net;
using System.Text.Json;
using Bogus;
using Microsoft.Extensions.Logging;
using N8nWebhookClient.Services;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace N8nWebhookClient.UnitTests.Services;

[TestFixture]
public class N8nWebhookServiceTests
{
    private N8NWebhookService _sut = null!;
    private HttpClient _httpClient = null!;
    private TestHttpMessageHandler _mockHandler = null!;
    private ILogger<N8NWebhookService> _logger = null!;
    private Faker _faker = null!;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
        _logger = Substitute.For<ILogger<N8NWebhookService>>();
        _mockHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_mockHandler);
        _sut = new N8NWebhookService(_httpClient, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
        _mockHandler?.Dispose();
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnSuccessResponse_WhenRequestSucceeds()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var expectedResponse = new TestResponseModel
        {
            Id = _faker.Random.Guid().ToString(),
            Message = _faker.Lorem.Sentence()
        };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data!.Id.ShouldBe(expectedResponse.Id);
        result.Data.Message.ShouldBe(expectedResponse.Message);
        result.RawResponse.ShouldNotBeNullOrEmpty();
        result.ErrorMessage.ShouldBeNull();
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnFailureResponse_WhenRequestFails()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var errorMessage = _faker.Lorem.Sentence();

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorMessage)
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Data.ShouldBeNull();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage!.ShouldContain("BadRequest");
        result.RawResponse.ShouldBe(errorMessage);
    }

    [Test]
    [TestCase(HttpStatusCode.NotFound, 404)]
    [TestCase(HttpStatusCode.InternalServerError, 500)]
    [TestCase(HttpStatusCode.Unauthorized, 401)]
    [TestCase(HttpStatusCode.Forbidden, 403)]
    [TestCase(HttpStatusCode.ServiceUnavailable, 503)]
    public async Task TriggerWebhookAsync_ShouldHandleDifferentStatusCodes_Correctly(
        HttpStatusCode statusCode,
        int expectedStatusCode)
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent("Error")
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.Success.ShouldBeFalse();
        result.StatusCode.ShouldBe(expectedStatusCode);
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldHandleHttpRequestException_Gracefully()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var exceptionMessage = _faker.Lorem.Sentence();

        _mockHandler.ExceptionToThrow = new HttpRequestException(exceptionMessage);

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exceptionMessage);
        result.StatusCode.ShouldBe(0);
        result.Data.ShouldBeNull();
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldHandleJsonException_WhenDeserializationFails()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var invalidJson = "{ invalid json content";

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(invalidJson)
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
        result.Data.ShouldBeNull();
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldSerializePayload_Correctly()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new ComplexPayload
        {
            Id = _faker.Random.Int(1, 1000),
            Name = _faker.Name.FullName(),
            Email = _faker.Internet.Email(),
            CreatedAt = DateTime.UtcNow,
            Tags = new List<string> { _faker.Lorem.Word(), _faker.Lorem.Word() }
        };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TestResponseModel()))
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        var content = _mockHandler.CapturedRequestContent;
        content.ShouldNotBeNull();
        content!.ShouldContain(payload.Name);
        content.ShouldContain(payload.Email);
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldUseCorrectContentType()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TestResponseModel()))
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        var capturedRequest = _mockHandler.LastRequest;
        capturedRequest.ShouldNotBeNull();
        capturedRequest!.Content!.Headers.ContentType!.MediaType.ShouldBe("application/json");
        capturedRequest.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldDeserializeResponse_CaseInsensitively()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var jsonWithDifferentCase = @"{""iD"":""123"",""MESSAGE"":""test""}";

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonWithDifferentCase)
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.Success.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data!.Id.ShouldBe("123");
        result.Data.Message.ShouldBe("test");
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnRawResponse_WhenSuccessful()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var responseData = new TestResponseModel
        {
            Id = _faker.Random.Guid().ToString(),
            Message = _faker.Lorem.Sentence()
        };
        var rawResponse = JsonSerializer.Serialize(responseData);

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(rawResponse)
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.RawResponse.ShouldBe(rawResponse);
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnRawResponse_WhenFailed()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var errorResponse = "Error occurred";

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorResponse)
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.RawResponse.ShouldBe(errorResponse);
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldUsePostMethod()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TestResponseModel()))
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        var capturedRequest = _mockHandler.LastRequest;
        capturedRequest.ShouldNotBeNull();
        capturedRequest!.Method.ShouldBe(HttpMethod.Post);
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldSendToCorrectUrl()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TestResponseModel()))
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        var capturedRequest = _mockHandler.LastRequest;
        capturedRequest.ShouldNotBeNull();
        // HttpClient may add trailing slash, use StartsWith or TrimEnd
        capturedRequest!.RequestUri!.ToString().TrimEnd('/').ShouldBe(webhookUrl.TrimEnd('/'));
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldLogInformation_WhenSendingRequest()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TestResponseModel()))
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains($"Sending webhook request to: {webhookUrl}")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldLogError_WhenRequestFails()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error")
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Webhook request failed with status code: BadRequest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldLogHttpRequestException_WithCorrectMessage()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var exception = new HttpRequestException("Network error");

        _mockHandler.ExceptionToThrow = exception;

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("HTTP request error triggering webhook")),
            exception,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldLogJsonException_WithCorrectMessage()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json {{{")
        };

        // Act
        await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("JSON serialization/deserialization error triggering webhook")),
            Arg.Any<JsonException>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnExactErrorMessage_ForFailedRequest()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server error")
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ErrorMessage.ShouldBe("Request failed with status code: InternalServerError");
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnHttpExceptionMessage_WhenHttpRequestFails()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };
        var exceptionMessage = "Connection refused";
        var exception = new HttpRequestException(exceptionMessage);

        _mockHandler.ExceptionToThrow = exception;

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ErrorMessage.ShouldBe(exceptionMessage);
    }

    [Test]
    public async Task TriggerWebhookAsync_ShouldReturnJsonExceptionMessage_WhenDeserializationFails()
    {
        // Arrange
        var webhookUrl = _faker.Internet.Url();
        var payload = new { message = _faker.Lorem.Sentence() };

        _mockHandler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json {{{")
        };

        // Act
        var result = await _sut.TriggerWebhookAsync<TestResponseModel>(webhookUrl, payload);

        // Assert
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
        result.ErrorMessage.ShouldContain("invalid");
    }
}

// Test helper for HttpMessageHandler
public class TestHttpMessageHandler : HttpMessageHandler
{
    public HttpResponseMessage? ResponseToReturn { get; set; }
    public Exception? ExceptionToThrow { get; set; }
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? CapturedRequestContent { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;

        // Capture the content before it gets disposed
        if (request.Content != null)
        {
            CapturedRequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        return ResponseToReturn ?? new HttpResponseMessage(HttpStatusCode.OK);
    }
}

// Test models
public class TestResponseModel
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ComplexPayload
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}
