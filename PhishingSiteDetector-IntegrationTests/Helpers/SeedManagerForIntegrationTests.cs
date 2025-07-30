using Microsoft.AspNetCore.Identity;
using Microsoft.ML;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using Soccerity_API_IntegrationTests.Data.Database;

namespace PhishingSiteDetector_API_IntegrationTests.Helpers
{
    public static class SeedManagerForIntegrationTests
    {
        public static void Seed(ApplicationDbContext context)
        {
            CreateLanguages(context);
            CreateRoles(context);
            CreateUsers(context);
            CreateUsersRoles(context);
            CreateDataSets(context);
        }

        public static void CreateLanguages(ApplicationDbContext context)
        {
            List<Language> languages = [DBLanguage.EN, DBLanguage.PL];

            foreach (var language in languages)
            {
                if (!context.Languages.Any(a => a.Code == language.Code))
                {
                    context.Languages.Add(language);
                }
            }
            context.SaveChanges();
        }

        public static void CreateRoles(ApplicationDbContext context)
        {
            List<IdentityRole> roles = [DBRole.Admin];

            foreach (var role in roles)
            {
                if (!context.Roles.Any(a => a.Id == role.Id))
                {
                    context.Roles.Add(role);
                }
            }
            context.SaveChanges();
        }

        public static void CreateUsers(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var adminUser = DBAdmin.Account;
            adminUser.PasswordHash = hasher.HashPassword(adminUser, DBAdmin.Password);
            var secondAdminUser = DBSecondAdmin.Account;
            secondAdminUser.PasswordHash = hasher.HashPassword(secondAdminUser, DBSecondAdmin.Password);

            List<ApplicationUser> users = [adminUser, secondAdminUser];

            foreach (var user in users)
            {
                if (!context.Roles.Any(a => a.Id == user.Id))
                {
                    context.Users.Add(user);
                }
            }
            context.SaveChanges();
        }

        public static void CreateUsersRoles(ApplicationDbContext context)
        {
            List<IdentityUserRole<string>> usersRoles = [
                new IdentityUserRole<string> { UserId = DBAdmin.Account.Id, RoleId = DBRole.Admin.Id },
                new IdentityUserRole<string> { UserId = DBSecondAdmin.Account.Id, RoleId = DBRole.Admin.Id }
            ];

            foreach (var item in usersRoles)
            {
                if (!context.UserRoles.Any(a => a.UserId == item.UserId && a.RoleId == item.RoleId))
                {
                    context.UserRoles.Add(item);
                }
            }
            context.SaveChanges();
        }

        public static void CreateDataSets(ApplicationDbContext context)
        {
            var binDirectory = AppContext.BaseDirectory;

            var modelFolder = Path.Combine(binDirectory, "MLModels");
            var dataSetsFolder = Path.Combine(binDirectory, "DataSets");

            var testProjectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "PhishingSiteDetector-IntegrationTests"));
            var csvFilePath = Path.Combine(testProjectRoot, "Data", "Csv", "Phishing_Legitimate_full.csv");

            Directory.CreateDirectory(dataSetsFolder);
            Directory.CreateDirectory(modelFolder);

            for (int i = 0; i < 2; i++)
            {
                var dataSet = new DataSet
                {
                    Name = "Phishing_Legitimate_full.csv",
                    IsActiveDataSet = i == 0 ? true : false,
                    CreationUserId = DBAdmin.Account.Id,
                    CreationDate = DateTime.Now
                };

                context.DataSets.Add(dataSet);
                context.SaveChanges();
                var dataSetId = dataSet.Id;

                var targetCsvPath = Path.Combine(dataSetsFolder, $"{dataSetId}.csv");
                File.Copy(csvFilePath, targetCsvPath, overwrite: true);

                var mlContext = new MLContext();
                var data = mlContext.Data.LoadFromTextFile<CsvDTO>(targetCsvPath, hasHeader: true, separatorChar: ',');
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
                        nameof(CsvDTO.RandomString),
                        nameof(CsvDTO.IpAddress),
                        nameof(CsvDTO.DomainInSubdomains),
                        nameof(CsvDTO.DomainInPaths),
                        nameof(CsvDTO.HttpsInHostname),
                        nameof(CsvDTO.HostnameLength),
                        nameof(CsvDTO.PathLength),
                        nameof(CsvDTO.QueryLength),
                        nameof(CsvDTO.DoubleSlashInPath),
                        nameof(CsvDTO.EmbeddedBrandName)
                    ).Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(CsvDTO.ClassLabel)));

                var model = pipeline.Fit(split.TrainSet);

                var modelPath = Path.Combine(modelFolder, $"{dataSetId}.zip");
                mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);
            }
        }
    }
}