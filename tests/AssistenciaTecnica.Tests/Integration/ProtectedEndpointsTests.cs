using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AssistenciaTecnica.Tests.Integration;

public class ProtectedEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProtectedEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/Dashboard")]
    [InlineData("/Clientes")]
    [InlineData("/Aparelhos")]
    [InlineData("/OrdensServico")]
    [InlineData("/Produtos")]
    [InlineData("/Estoque")]
    [InlineData("/Configuracoes")]
    [InlineData("/Agendamentos")]
    [InlineData("/Orcamentos")]
    [InlineData("/Tecnicos")]
    [InlineData("/Relatorios")]
    public async Task EndpointsProtegidos_SemLogin_DeveRedirecionarParaLogin(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }
}
