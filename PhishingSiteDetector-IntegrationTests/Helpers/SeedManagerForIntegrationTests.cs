using Microsoft.AspNetCore.Identity;
using Microsoft.ML;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.Domain;
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
            foreach (var language in DBLanguages.All)
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
            foreach (var role in DBRoles.All)
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
                new IdentityUserRole<string> { UserId = DBAdmin.Account.Id, RoleId = DBRoles.Admin.Id },
                new IdentityUserRole<string> { UserId = DBSecondAdmin.Account.Id, RoleId = DBRoles.Admin.Id }
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
                    AddedByUserId = DBAdmin.Account.Id,
                    AddedDate = DateTime.Now
                };

                context.DataSets.Add(dataSet);
                context.SaveChanges();
                var dataSetId = dataSet.Id;

                var targetCsvPath = Path.Combine(dataSetsFolder, $"{dataSetId}.csv");
                File.Copy(csvFilePath, targetCsvPath, overwrite: true);

                var mlContext = new MLContext();
                var data = mlContext.Data.LoadFromTextFile<CsvColumns>(targetCsvPath, hasHeader: true, separatorChar: ',');
                var split = mlContext.Data.TrainTestSplit(data, 0.2);

                var pipeline = mlContext.Transforms.Concatenate("Features",
                        nameof(CsvColumns.NumDots),
                        nameof(CsvColumns.SubdomainLevel),
                        nameof(CsvColumns.PathLevel),
                        nameof(CsvColumns.UrlLength),
                        nameof(CsvColumns.NumDash),
                        nameof(CsvColumns.NumDashInHostname),
                        nameof(CsvColumns.AtSymbol),
                        nameof(CsvColumns.TildeSymbol),
                        nameof(CsvColumns.NumUnderscore),
                        nameof(CsvColumns.NumPercent),
                        nameof(CsvColumns.NumQueryComponents),
                        nameof(CsvColumns.NumAmpersand),
                        nameof(CsvColumns.NumHash),
                        nameof(CsvColumns.NumNumericChars),
                        nameof(CsvColumns.NoHttps),
                        nameof(CsvColumns.RandomString),
                        nameof(CsvColumns.IpAddress),
                        nameof(CsvColumns.DomainInSubdomains),
                        nameof(CsvColumns.DomainInPaths),
                        nameof(CsvColumns.HttpsInHostname),
                        nameof(CsvColumns.HostnameLength),
                        nameof(CsvColumns.PathLength),
                        nameof(CsvColumns.QueryLength),
                        nameof(CsvColumns.DoubleSlashInPath),
                        nameof(CsvColumns.EmbeddedBrandName)
                    ).Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(CsvColumns.ClassLabel)));

                var model = pipeline.Fit(split.TrainSet);

                var modelPath = Path.Combine(modelFolder, $"{dataSetId}.zip");
                mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);
            }
        }
    }
}