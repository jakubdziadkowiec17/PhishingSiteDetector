using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API_IntegrationTests.Helpers;
using Soccerity_API_IntegrationTests.Data.Database;
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
            var tokensForRefreshDTO = new TokensForRefreshDTO { AccessToken = tokensDTO.AccessToken, RefreshToken = tokensDTO.RefreshToken };

            var response = await _httpClient.PostAsJsonAsync($"/api/account/refresh-tokens", tokensForRefreshDTO);

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
            var accountDTO = new AccountDTO { FirstName = DBAdmin.Account.FirstName, LastName = DBAdmin.Account.LastName, Email = DBAdmin.Account.Email, DateOfBirth = DBAdmin.Account.DateOfBirth, Address = DBAdmin.Account.Address, AreaCode = DBAdmin.Account.AreaCode, PhoneNumber = DBAdmin.Account.PhoneNumber };

            var response = await _httpClient.PutAsJsonAsync("/api/account", accountDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ChangeLanguage_ValidLanguageData_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var accountDTO = new LanguageDTO { LanguageCode = LanguageCode.PL };

            var response = await _httpClient.PutAsJsonAsync("/api/account/change-language", accountDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_ValidResetPasswordData_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBSecondAdmin.Account.Email, DBSecondAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var resetPasswordDTO = new ResetPasswordDTO { OldPassword = DBSecondAdmin.Password, NewPassword = "Admin33!", ConfirmNewPassword = "Admin33!" };

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

        //DATASET CONTROLLER

        [Fact]
        public async Task UploadDataSet_ValidCsvFile_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var testProjectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "PhishingSiteDetector-IntegrationTests"));
            var filePath = Path.Combine(testProjectRoot, "Data", "Csv", "Phishing_Legitimate_full.csv");
            await using var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            var dataSetDTO = new MultipartFormDataContent
            {
                { fileContent, "File", "Phishing_Legitimate_full.csv" },
                { new StringContent("false"), "IsActiveDataSet" }
            };

            var response = await _httpClient.PostAsync("/api/data-set", dataSetDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDataSets_DataSetsExists_ShouldReturnOkAndList()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var pageNumber = 1;
            var pageSize = 10;

            var response = await _httpClient.GetAsync($"/api/data-set?pageNumber={pageNumber}&pageSize={pageSize}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<ListPageDTO<DataSetItemDTO>>();
            Assert.NotNull(data);
            Assert.True(data.Items.Count >= 0);
        }

        [Fact]
        public async Task DownloadDataSet_DataSetExists_ShouldReturnOkAndFile()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var id = 1;

            var response = await _httpClient.GetAsync($"/api/data-set/download/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task UpdateActivityForDataSet_DataSetExists_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var id = 1;
            var dataSetItemDTO = new DataSetItemDTO
            {
                Id = 1,
                Name = "Phishing_Legitimate_full.csv",
                IsActiveDataSet = true,
                CreationUserId = DBAdmin.Account.Id,
                CreationDate = DateTime.Now
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/data-set/{id}", dataSetItemDTO);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDataSet_DataSetExists_ShouldReturnOk()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var id = 2;

            var response = await _httpClient.DeleteAsync($"/api/data-set/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //SITELOG CONTROLLER

        [Fact]
        public async Task GetSiteLogs_SiteLogsExists_ShouldReturnOkAndChartDTO()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var year = DateTime.Now.Year;

            var response = await _httpClient.GetAsync($"/api/site-log?year={year}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var chartData = await response.Content.ReadFromJsonAsync<List<ChartDTO>>();
            Assert.NotNull(chartData);
        }

        //URLPREDICTION CONTROLLER

        [Fact]
        public async Task Predict_ValidUrl_ShouldReturnOkAndUrlPredictionDTO()
        {
            var tokensDTO = await Tools.LoginAsync(_httpClient, DBAdmin.Account.Email, DBAdmin.Password);
            _httpClient = Tools.GetDefaultRequestHeaders(_httpClient, tokensDTO);
            var urlDto = new UrlDTO { Url = "https://temporary-site-for-tests.com/" };

            var response = await _httpClient.PostAsJsonAsync("/api/url-prediction", urlDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<UrlPredictionDTO>();
            Assert.NotNull(result);
        }
    }
}