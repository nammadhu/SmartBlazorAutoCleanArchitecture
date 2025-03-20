using CleanArchitecture.Infrastructure.Persistence.Contexts;
using CleanArchitecture.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace CleanArchitecture.WebApi;

public class Seed
    {
    public static async Task Seeding(WebApplication app)
        {
        using (var scope = app.Services.CreateScope())
            {
            var services = scope.ServiceProvider;
#if AspNetIdentity
            //this is for identity database & tables creation
            await services.GetRequiredService<IdentityContext>().Database.MigrateAsync();
            await services.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
#endif
            //disabling fileManagerDbContext as not using,so db will not be created.To enable had to to in this program.cs at 2 place here & above at services
            // await services.GetRequiredService<FileManagerDbContext>().Database.MigrateAsync();

            //below is for data loading to table
            bool loadDataToTable = true;
            if (loadDataToTable)
                {
#if AspNetIdentity
                await SeedIdentity.SeedRolesAsync(services.GetRequiredService<RoleManager<ApplicationRole>>());
                await SeedIdentity.SeedDefaultUsersAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
#endif

                await DefaultCardTypeSeeds.SeedCardTypes(
                    services.GetRequiredService<DbContextProvider>().DbContext);

                //currently loading town from script only,as its list is more
                //DefaultDataTownSeeds.Seed(services.GetRequiredService<ApplicationDbContext>());//this not using now

                var dbContextProvider = services.GetRequiredService<DbContextProvider>();
                var context = dbContextProvider.DbContext;

                if (!await context.Towns.AnyAsync())
                    {
                    var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"bin\Debug\net9.0\Seeds\scriptAllIndiaTownsSeedSqlScripttoInsert.sql");

                    if (File.Exists(sqlFilePath))
                        {
                        var sql = await File.ReadAllTextAsync(sqlFilePath);
                        await context.Database.ExecuteSqlRawAsync(sql);
                        }
                    else
                        {
                        Console.WriteLine($"File not found: {sqlFilePath}");
                        }
                    }
                dbContextProvider.Dispose();
                }
            }
        }
    }
