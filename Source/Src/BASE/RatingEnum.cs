﻿namespace BASE;
//https://sansad.in/

//stop using enum & instead of this use class V_KPI
//public enum KPIEnum//this can be moved to db and read once & store in cache or static but later //TODO
//{
//    [Display(Name = "Administration & TransparentPolitics", Description = "3 is best,-2 is very bad full of curruption")]
//    AdministrationTransparentPolitics = 1,//    AdministrationTransparentPolitics,Corruption

//    HealthSectorQuality = 2,
//    EducationSectorGovt = 3,//EducationSectorPrivate,
//    IndustryDevelopmentExisitng = 4,
//    IndustryBringingNew = 5,
//    EmploymentCreationLocal = 6,
//    RoadsAndInfrastructureDevelopment = 7,

//    PublicTransport = 8,

//    AgriculturalSupport = 9,
//    EnvironmentalSafety = 10,
//    RiotsAndSafetyControl = 11,
//    FamilyAndCastePolitics = 12
//}

//public enum RatingEnum
//{
//    VeryBad =  -2,
//    Bad =  -1,
//    OkOk = 0,
//    //GoodPersonButUnableToDoDueToOtherCircumstances = 1,
//    GoodWork =  1,
//    GreatWork =  2
//}
public enum RatingEnum
    {
    VeryBad = 1,// -2,
    Bad = 2,// -1,
    OkOk = 3,//0,

    //GoodPersonButUnableToDoDueToOtherCircumstances = 1,
    GoodWork = 4,// 1,

    GreatWork = 5,// 2
    }