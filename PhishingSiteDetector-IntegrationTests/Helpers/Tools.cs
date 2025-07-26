using PhishingSiteDetector_API.Models.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace PhishingSiteDetector_API_IntegrationTests.Helpers
{
    public class Tools
    {
        public static async Task<TokensDTO> LoginAsync(HttpClient httpClient, string email, string password)
        {
            var loginDTO = new LoginDTO { Email = email, Password = password };

            var response = await httpClient.PostAsJsonAsync($"/api/account/login", loginDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tokensDTO = await response.Content.ReadFromJsonAsync<TokensDTO>();
            Assert.NotNull(tokensDTO);
            
            return tokensDTO;
        }

        public static HttpClient GetDefaultRequestHeaders(HttpClient httpClient, TokensDTO tokensDTO)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokensDTO.AccessToken);
            
            return httpClient;
        }
    }
}