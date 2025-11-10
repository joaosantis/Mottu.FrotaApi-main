using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Mottu.FrotaApi.Models;

namespace Mottu.FrotaApi.Tests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // Helper: Faz login e retorna token JWT
        private async Task<string> ObterTokenAsync()
        {
            var loginJson = JsonSerializer.Serialize(new
            {
                Username = "admin",
                Password = "123"
            });

            var response = await _client.PostAsync("/api/v1/auth/login",
                new StringContent(loginJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("token").GetString()!;
        }

        // ==================== TESTES DE AUTENTICAÇÃO ====================
        [Fact]
        public async Task Login_Sucesso_RetornaToken()
        {
            var loginJson = JsonSerializer.Serialize(new
            {
                Username = "admin",
                Password = "123"
            });

            var response = await _client.PostAsync("/api/v1/auth/login",
                new StringContent(loginJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", body);
        }

        // ==================== TESTES DE FILIAIS ====================
        [Fact]
        public async Task Filiais_Get_RetornaOk()
        {
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/filiais");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ==================== TESTES DE MANUTENÇÕES ====================
        [Fact]
        public async Task Manutencoes_Get_RetornaOk()
        {
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/manutencoes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ==================== TESTES DE MOTO ====================
        // Teste unitário simples
        [Fact]
        public void Deve_Criar_Moto_Com_Valores_Validos()
        {
            var moto = new Moto
            {
                Placa = "XYZ-1234",
                Modelo = "Honda CG",
                Status = "Ativa",
                FilialId = 1
            };

            Assert.Equal("XYZ-1234", moto.Placa);
            Assert.Equal("Honda CG", moto.Modelo);
            Assert.Equal("Ativa", moto.Status);
            Assert.Equal(1, moto.FilialId);
        }

        // Teste de integração com autenticação
        [Fact]
        public async Task Motos_Get_RetornaOk()
        {
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/motos");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ==================== TESTES DE MACHINE LEARNING ====================
        [Fact]
        public async Task ML_PreverDisponibilidade_RetornaOk()
        {
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var inputJson = JsonSerializer.Serialize(new
            {
                KmRodados = 12000,
                TempoUsoMeses = 12,
                FilialId = 1
            });

            var response = await _client.PostAsync("/api/v1/ml/prever-disponibilidade",
                new StringContent(inputJson, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("disponivel", body);
            Assert.Contains("probabilidade", body);
        }
    }
}
