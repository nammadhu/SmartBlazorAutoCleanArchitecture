namespace CleanArchitecture.Infrastructure.Persistence.Seeds;

//currently not inserting this,instead using sql file or script to manually load by turning off identity and then on
public static class DefaultDataTownSeeds
{
    public static void Seed(ApplicationDbContext applicationDbContext)
    {
        //instead of this use scriptAllIndiaTownsSeedSqlScripttoInsert.sql script and manually execute

        //currently not inserting this,instead using sql file or script to manually load by turning off identity and then on
        // return;//if you want to skip then uncomment this return statement;
        //here dont use async... it makes 2 threads blocking for same object sometimes

        Console.WriteLine("Seed Towns data loading");//

        /////////////////towns adding

        var existingTowns = applicationDbContext.Towns.ToList();

        List<Town> townsMasterData = [
                new Town("Bhadravathi"){Id=1, UrlName1="Bhadravathi.com",UrlName2= "Bdvt.in"
                    ,District="Shivamogga",State="Karnataka",SubTitle="Iron & Steel Rusting Towns"
                    ,Latitude=13.84312,Longitude=75.695900
                   // TownCardVerified=new Card_VerifiedEntries(){ IdCardType=ConstantsTown.TownTypeId,IdTown=1,
                   // Name="Bhadravathi",SubTitle="Iron & Steel Rusting Towns",Active=true,
                   ////OtherReferenceUrl="https://en.wikipedia.org/wiki/Bhadravati,_Karnataka",GoogleMapAddressUrl="https://www.google.com/maps/place/Bhadravathi,+Karnataka/@13.8425707,75.692868,14.02z/data=!4m6!3m5!1s0x3bbb0004bae5616f:0x5eab5b9250ba013e!8m2!3d13.8275718!4d75.7063783!16zL20vMDh3aGpx?entry=ttu",
                   },
                new Town("Kadur"){ Id=2, UrlName1="Kadur.in",Latitude=13.554609,Longitude=76.013241},
                new Town("Birur"){ Id=3, UrlName1="Birur.in",Latitude=13.597312,Longitude=75.973205},
                new Town("Tarikere"){ Id=4,UrlName1="Tarikere.in",Latitude=13.711352,Longitude=75.810555},
                new Town("Arsikere"){ Id = 5, UrlName1 = "Arsikere.in",Latitude=13.313620,Longitude=76.256408},
                new Town("Honnavara"){Id=6, UrlName1="Honnavar.in",Latitude=14.279788,Longitude=74.445264},
                new Town("Shivamogga"){Id=7, UrlName1="Shmg.in",Latitude=13.929909,Longitude=75.568368},
                new Town("Hoskote"){Id=8, UrlName1="Hoskote.in",Latitude=13.0696026,Longitude=77.7972733},
                ];
        townsMasterData.ForEach(x => x.Name = x.Name);
        Console.WriteLine($"existingTowns count {existingTowns.Count},towns count {townsMasterData.Count} Starting");
        int changesCount = 0;
        int addedTowns = 0;
        if (existingTowns != null && existingTowns.Count > 0)
        {
            Console.WriteLine($"Adding difference towns ");
            townsMasterData.ForEach(town =>
            {
                //instead do comparison on name & district both together
                if (!existingTowns.Exists(e => e.Name == town.Name && e.State == town.State))
                {
                    town.Id = 0;
                    var townAddedEntity = applicationDbContext.Towns.Add(town);
                    changesCount += applicationDbContext.SaveChangesAsync().Result;
                    addedTowns++;
                }
            });
        }
        else
        {
            //make all townid as 0 and insert
            Console.WriteLine($"Adding {townsMasterData.Count} Towns");
            townsMasterData.ForEach(x => x.Id = 0);
            var townAddedEntity = applicationDbContext.Towns.AddRangeAsync(townsMasterData);
            changesCount += applicationDbContext.SaveChangesAsync().Result;
            Console.WriteLine($"Added {townsMasterData.Count} Towns Success ");
        }
        Console.WriteLine($"{addedTowns} Towns added with {changesCount} changes");
    }
}
