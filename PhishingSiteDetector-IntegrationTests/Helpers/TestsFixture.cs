using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API_IntegrationTests.Helpers;

namespace PhishingSiteDetector_API_IntegrationTests
{
    public class TestsFixture : IDisposable
    {
        public WebApplicationFactory<Program> WebApplicationFactory { get; set; }
        public HttpClient HttpClient { get; set; }

        public TestsFixture()
        {
            WebApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceDescriptor = services.SingleOrDefault(a => a.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (serviceDescriptor != null)
                    {
                        services.Remove(serviceDescriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("PhishingSiteDetector-DB-Tests");
                    });

                    using (var scope = services.BuildServiceProvider().CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        context.Database.EnsureCreated();
                        SeedManagerForIntegrationTests.Seed(context);
                    }
                });
            });
            HttpClient = WebApplicationFactory.CreateClient();
        }

        public void Dispose()
        {
            WebApplicationFactory.Dispose();
        }
    }
}