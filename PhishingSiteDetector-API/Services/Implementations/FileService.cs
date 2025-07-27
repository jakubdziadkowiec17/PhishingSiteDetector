using Microsoft.ML;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class FileService : IFileService
    {
        public FileService() {}

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null)
            {
                throw new Exception(ERROR.FILE_IS_EMPTY);
            }
            if (!file.FileName.EndsWith(".csv"))
            {
                throw new Exception(ERROR.FILE_SHOULD_BE_CSV);
            }

            var folderPath = Path.Combine("DataSets");
            Directory.CreateDirectory(folderPath);

            foreach (var existingFile in Directory.GetFiles(folderPath))
            {
                File.Delete(existingFile);
            }

            var filePath = Path.Combine(folderPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Take(3))
            {
                Console.WriteLine(line);
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

            var model = pipeline.Fit(split.TrainSet);

            var transformedTestSet = model.Transform(split.TestSet);

            mlContext.Model.Save(model, split.TrainSet.Schema, "MLModels/PhishingModel.zip");

            return (INFO.FILE_ADDED);
        }
    }
}