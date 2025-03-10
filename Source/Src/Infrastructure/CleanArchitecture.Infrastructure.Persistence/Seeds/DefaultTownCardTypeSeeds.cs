using Shared;

namespace CleanArchitecture.Infrastructure.Persistence.Seeds;

//currently not inserting this,instead using sql file or script to manually load by turning off identity and then on
public static class DefaultTownCardTypeSeeds
{
    public static async Task SeedCardTypes(ApplicationDbContext applicationDbContext)
    {
        // return;//if you want to skip then uncomment this return statement;
        //here dont use async... it makes 2 threads blocking for same object sometimes

        //Console.WriteLine("Seed data loading,icons loading from https://pictogrammers.com/library/mdi/icon/hospital-box/");
        //https://pictogrammers.com/library/mdi/icon/hospital-box/

        #region cardType

        var existingCardTypes = applicationDbContext.CardTypes.ToList();

        int count = 0;
        Console.WriteLine($"existingCardTypes count {existingCardTypes.Count},cardTypes count {ConstantsTown.cardTypes.Count}");
        if (existingCardTypes != null && existingCardTypes.Count > 0)
        {
            // cardTypes.ForEach(async cardType =>//dont use this instead use below normal for
            foreach (var cardType in ConstantsTown.cardTypes)
            {
                if (!existingCardTypes.Exists(e => e.Id == cardType.Id))
                {
                    applicationDbContext.CardTypes.Add(cardType);
                    count += await applicationDbContext.SaveChangesAsync();
                }
            }
            Console.WriteLine($"{count} CardTypes added");
        }
        else
        {
            applicationDbContext.CardTypes.AddRange(ConstantsTown.cardTypes);
            count = await applicationDbContext.SaveChangesAsync();
            Console.WriteLine($"{count} CardTypes added");
        }

        #endregion cardType
    }
}
