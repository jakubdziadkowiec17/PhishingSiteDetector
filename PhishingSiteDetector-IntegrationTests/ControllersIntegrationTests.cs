using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API_IntegrationTests.Helpers;
using Soccerity_API_IntegrationTests.Database.Data;
using System.Net;
using System.Net.Http.Json;

namespace PhishingSiteDetector_API_IntegrationTests.Controllers
{
    public class ControllersIntegrationTests : IClassFixture<TestsFixture>
    {
        private HttpClient _httpClient;

        public ControllersIntegrationTests(TestsFixture testsFixture)
        {
            _httpClient = testsFixture.HttpClient;
        }

        //ACCOUNT CONTROLLER
        [Fact]
        public async Task Login_ValidUserData_ShouldReturnOkAndTokens()
        {
            await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
        }

        [Fact]
        public async Task Login_InvalidUserData_ShouldReturnUnauthorized()
        {
            var loginDTO = new LoginDTO { Email = DBAdmin.Account.Email, Password = Guid.NewGuid().ToString() };
            var languageCode = LanguageCode.EN;

            var response = await _httpClient.PostAsJsonAsync($"/api/account/login", loginDTO);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RefreshTokens_ValidUserData_ShouldReturnOkAndTokens()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            var languageCode = LanguageCode.EN;

            var response = await _httpClient.PostAsJsonAsync($"/api/account/refresh-tokens", tokensDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<TokensDTO>();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task GetAccountData_UserExists_ShouldReturnOkAndAccountData()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);

            var response = await _httpClient.GetAsync("/api/account/data");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<AccountDataDTO>();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task GetAccount_UserExists_ShouldReturnOkAndAccount()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);

            var response = await _httpClient.GetAsync("/api/account");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task EditAccount_ValidAccountData_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var accountDTO = new AccountDTO { FirstName = DBAdmin.Account.FirstName, LastName = DBAdmin.Account.LastName, Email = DBAdmin.Account.Email, DateOfBirth = DBAdmin.Account.DateOfBirth, LanguageCode = LanguageCode.PL, Address = DBAdmin.Account.Address, AreaCode = DBAdmin.Account.AreaCode, PhoneNumber = DBAdmin.Account.PhoneNumber };

            var response = await _httpClient.PutAsJsonAsync("/api/account", accountDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_ValidResetPasswordData_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBSecondAdmin.Account.Email, DBSecondAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var resetPasswordDTO = new ResetPasswordDTO { OldPassword = DBSecondAdmin.Password, NewPassword = "Admin33!", ConfirmNewPassword = "Admin22!" };

            var response = await _httpClient.PutAsJsonAsync("/api/account/reset-password", resetPasswordDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Logout_ValidRefreshTokenData_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var refreshTokenDTO = new RefreshTokenDTO { RefreshToken = tokensDTO.RefreshToken };

            var response = await _httpClient.PostAsJsonAsync("/api/account/logout", refreshTokenDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}