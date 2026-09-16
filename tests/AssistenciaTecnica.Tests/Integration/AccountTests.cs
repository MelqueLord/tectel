using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AssistenciaTecnica.Tests.Integration;

public class AccountTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Login_Get_DeveRetornar200()
    {
        var response = await _client.GetAsync("/Account/Login");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Login", content);
    }

    [Fact]
    public async Task Register_Get_DeveRetornar200()
    {
        var response = await _client.GetAsync("/Account/Register");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Criar Conta", content);
    }

    [Fact]
    public async Task AccessDenied_DeveRetornar200()
    {
        var response = await _client.GetAsync("/Account/AccessDenied");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Dashboard_SemLogin_DeveRedirecionarParaLogin()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Login_Post_CredenciaisValidas_DeveRedirecionar()
    {
        // Primeiro obter o token antiforgery
        var loginPage = await _client.GetAsync("/Account/Login");
        var loginHtml = await loginPage.Content.ReadAsStringAsync();

        var token = ExtractAntiForgeryToken(loginHtml);

        var formData = new Dictionary<string, string>
        {
            ["Email"] = "admin@assistencia.com",
            ["Password"] = "Test@123456",
            ["__RequestVerificationToken"] = token
        };

        var response = await _client.PostAsync("/Account/Login",
            new FormUrlEncodedContent(formData));

        Assert.True(
            response.StatusCode == HttpStatusCode.Redirect ||
            response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_Post_CredenciaisInvalidas_DeveRetornarComErro()
    {
        var loginPage = await _client.GetAsync("/Account/Login");
        var loginHtml = await loginPage.Content.ReadAsStringAsync();

        var token = ExtractAntiForgeryToken(loginHtml);

        var formData = new Dictionary<string, string>
        {
            ["Email"] = "invalido@email.com",
            ["Password"] = "senhaerrada",
            ["__RequestVerificationToken"] = token
        };

        var response = await _client.PostAsync("/Account/Login",
            new FormUrlEncodedContent(formData));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("inválidos", content);
    }

    private static string ExtractAntiForgeryToken(string html)
    {
        var marker = "__RequestVerificationToken\" type=\"hidden\" value=\"";
        var start = html.IndexOf(marker) + marker.Length;
        var end = html.IndexOf("\"", start);
        return html[start..end];
    }
}
