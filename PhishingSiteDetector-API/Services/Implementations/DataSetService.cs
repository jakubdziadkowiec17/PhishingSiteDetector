using AutoMapper;
using Microsoft.ML;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Domain;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;
using PhishingSiteDetector_API.Services.Interfaces;
using System.Security.Claims;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class DataSetService : IDataSetService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataSetRepository _dataSetRepository;
        private readonly IMapper _mapper;
        public DataSetService(IHttpContextAccessor httpContextAccessor, IDataSetRepository dataSetRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataSetRepository = dataSetRepository;
            _mapper = mapper;
        }

        public async Task<string> UploadAsync(DataSetDTO dataSetDTO)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (dataSetDTO is null || dataSetDTO.File is null)
            {
                throw new Exception(ERROR.DATA_SET_IS_EMPTY);
            }
            if (!dataSetDTO.File.FileName.EndsWith(".csv"))
            {
                throw new Exception(ERROR.DATA_SET_SHOULD_BE_CSV);
            }

            var dataSets = await _dataSetRepository.GetDataSetsAsync();
            foreach (var item in dataSets)
            {
                item.IsActiveDataSet = false;
                await _dataSetRepository.UpdateActivityForDataSetAsync(item);
            }

            var dataSet = new DataSet
            {
                Name = dataSetDTO.File.FileName,
                IsActiveDataSet = true,
                CreationUserId = userId,
                CreationDate = DateTime.Now
            };
            var dataSetId = await _dataSetRepository.CreateDataSetAsync(dataSet);

            var folderPath = Path.Combine("DataSets");
            Directory.CreateDirectory(folderPath);
            var fileExtension = Path.GetExtension(dataSetDTO.File.FileName);
            var filePath = Path.Combine(folderPath, $"{dataSetId}{fileExtension}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dataSetDTO.File.CopyToAsync(stream);
            }

            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<CsvDTO>(filePath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, 0.2);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    nameof(CsvDTO.NumDots),
                    nameof(CsvDTO.SubdomainLevel),
                    nameof(CsvDTO.PathLevel),
                    nameof(CsvDTO.UrlLength),
                    nameof(CsvDTO.NumDash),
                    nameof(CsvDTO.NumDashInHostname),
                    nameof(CsvDTO.AtSymbol),
                    nameof(CsvDTO.TildeSymbol),
                    nameof(CsvDTO.NumUnderscore),
                    nameof(CsvDTO.NumPercent),
                    nameof(CsvDTO.NumQueryComponents),
                    nameof(CsvDTO.NumAmpersand),
                    nameof(CsvDTO.NumHash),
                    nameof(CsvDTO.NumNumericChars),
                    nameof(CsvDTO.NoHttps),
                    nameof(CsvDTO.IpAddress),
                    nameof(CsvDTO.HttpsInHostname),
                    nameof(CsvDTO.HostnameLength),
                    nameof(CsvDTO.PathLength),
                    nameof(CsvDTO.QueryLength),
                    nameof(CsvDTO.DoubleSlashInPath)
                ).Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(CsvDTO.ClassLabel)));

            var modelFolder = "MLModels";
            Directory.CreateDirectory(modelFolder);
            var modelPath = Path.Combine(modelFolder, $"{dataSetId}.zip");
            var model = pipeline.Fit(split.TrainSet);
            mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);

            return (INFO.DATA_SET_ADDED);
        }

        public async Task<ListPageDTO<DataSetItemDTO>> GetDataSetsAsync(string? searchText, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(searchText)) searchText = "";
            var dataSetListPageDTO = await _dataSetRepository.GetDataSetsAsync(searchText, pageNumber, pageSize);
            var dataSets = _mapper.Map<List<DataSetItemDTO>>(dataSetListPageDTO.Items);

            return new ListPageDTO<DataSetItemDTO>(dataSets, dataSetListPageDTO.Count, dataSetListPageDTO.PageNumber);
        }

        public async Task<FileData> DownloadDataSetAsync(int id)
        {
            var dataSet = await _dataSetRepository.GetDataSetAsync(id);
            if (dataSet is null)
            {
                throw new Exception(ERROR.DATA_SET_NOT_FOUND);
            }

            var filePath = Path.Combine("DataSets", $"{id}.csv");

            if (!File.Exists(filePath))
            {
                throw new Exception(ERROR.DATA_SET_NOT_FOUND);
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            return new FileData
            {
                Name = dataSet.Name,
                Content = fileBytes
            };
        }

        public async Task<string> UpdateActivityForDataSetAsync(int id, DataSetItemDTO dataSetItemDTO)
        {
            var dataSet = await _dataSetRepository.GetDataSetAsync(id);
            if (dataSet is null)
            {
                throw new Exception(ERROR.DATA_SET_NOT_FOUND);
            }

            dataSet.IsActiveDataSet = dataSetItemDTO.IsActiveDataSet;

            await _dataSetRepository.UpdateActivityForDataSetAsync(dataSet);

            return INFO.DATA_SET_ACTIVITY_UPDATED;
        }

        public async Task<string> DeleteDataSetAsync(int id)
        {
            var dataSet = await _dataSetRepository.GetDataSetAsync(id);
            if (dataSet is null)
            {
                throw new Exception(ERROR.DATA_SET_NOT_FOUND);
            }

            await _dataSetRepository.DeleteDataSetAsync(dataSet);

            var dataSetPath = Path.Combine("DataSets", $"{id}.csv");
            if (File.Exists(dataSetPath))
            {
                File.Delete(dataSetPath);
            }

            var modelPath = Path.Combine("MLModels", $"{id}.zip");
            if (File.Exists(modelPath))
            {
                File.Delete(modelPath);
            }

            return INFO.DATA_SET_DELETED;
        }
    }
}