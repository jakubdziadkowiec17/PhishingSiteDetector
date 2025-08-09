using AutoMapper;
using Microsoft.ML;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Domain;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class UrlPredictionService : IUrlPredictionService
    {
        private readonly IDataSetRepository _dataSetRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly IMapper _mapper;
        public UrlPredictionService(IDataSetRepository dataSetRepository, ISiteLogRepository siteLogRepository, IMapper mapper)
        {
            _dataSetRepository = dataSetRepository;
            _siteLogRepository = siteLogRepository;
            _mapper = mapper;
        }

        public async Task<UrlPredictionDTO> PredictAsync(UrlDTO urlDTO)
        {
            var activeDataSet = await _dataSetRepository.GetActiveDataSetAsync();
            if (activeDataSet is null)
            {
                throw new Exception(ERROR.NO_ACTIVE_DATA_SET_FOUND);
            }

            var _mlContext = new MLContext();
            var _model = _mlContext.Model.Load($"MLModels/{activeDataSet.Id}.zip", out _);
            var _engine = _mlContext.Model.CreatePredictionEngine<UrlFeatures, UrlPrediction>(_model);

            var uri = new UriBuilder(urlDTO.Url).Uri;

            var features = new UrlFeatures
            {
                UrlLength = urlDTO.Url.Length,
                NumDots = urlDTO.Url.Count(c => c == '.'),
                SubdomainLevel = uri.Host.Split('.').Length - 2 > 0 ? uri.Host.Split('.').Length - 2 : 0,
                PathLevel = uri.AbsolutePath.Count(c => c == '/'),
                NumDash = urlDTO.Url.Count(c => c == '-'),
                NumDashInHostname = uri.Host.Count(c => c == '-'),
                AtSymbol = urlDTO.Url.Contains('@') ? 1 : 0,
                TildeSymbol = urlDTO.Url.Contains('~') ? 1 : 0,
                NumUnderscore = urlDTO.Url.Count(c => c == '_'),
                NumPercent = urlDTO.Url.Count(c => c == '%'),
                NumQueryComponents = uri.Query.Count(c => c == '&') + (string.IsNullOrWhiteSpace(uri.Query) ? 0 : 1),
                NumAmpersand = urlDTO.Url.Count(c => c == '&'),
                NumHash = urlDTO.Url.Count(c => c == '#'),
                NumNumericChars = urlDTO.Url.Count(char.IsDigit),
                NoHttps = urlDTO.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? 0 : 1,
                RandomString = urlDTO.IsRandomString ? 1 : 0,
                IpAddress = Uri.CheckHostName(uri.Host) == UriHostNameType.IPv4 ? 1 : 0,
                DomainInSubdomains = urlDTO.HasDomainInSubdomain ? 1 : 0,
                DomainInPaths = urlDTO.HasDomainInPath ? 1 : 0,
                HttpsInHostname = uri.Host.Contains("https") ? 1 : 0,
                HostnameLength = uri.Host.Length,
                PathLength = uri.AbsolutePath.Length,
                QueryLength = uri.Query.Length,
                DoubleSlashInPath = uri.AbsolutePath.Contains("//") ? 1 : 0,
                EmbeddedBrandName = urlDTO.HasEmbeddedBrandName ? 1 : 0,
            };

            var prediction = _engine.Predict(features);

            var siteLog = new SiteLog
            {
                AddedDate = DateTime.Now
            };

            await _siteLogRepository.CreateSiteLogAsync(siteLog);

            return _mapper.Map<UrlPredictionDTO>(prediction);
        }
    }
}