
using Anonymazer.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Anonymazer.Tests;

public class AuthorizationTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthorizationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    private string GenerateToken(string role)
    {
        var configuration = _factory.Services
            .GetRequiredService<IConfiguration>();

        var jwtKey = configuration["Jwt:Key"]!;
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.Role, role)
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task Anonymization_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync(
            "/api/Anonymization",
            new StringContent("""{"text":"test@example.com"}""",
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminStatistics_UserRole_ReturnsForbidden()
    {
        var token = GenerateToken("User");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Admin/statistics");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminStatistics_AdminRole_ReturnsOk()
    {
        var token = GenerateToken("Admin");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Admin/statistics");

        var authInfo = response.Headers.WwwAuthenticate?.ToString()
    ?? "Заголовок WWW-Authenticate отсутствует";

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"Статус: {response.StatusCode}\n" +
            $"Аутентификация: {authInfo}\n" +
            $"Ответ: {body}");

    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
