﻿namespace PublicCommon;

public static class ConstantString
    {
    public const string CONSTANTSTRINGRESOURCEID =
        "CleanArchitecture.Blazor.Infrastructure.Resources.Constants.ConstantString";

    //private static readonly ResourceManager rm;

    //static ConstantString()
    //    {
    //    rm = new ResourceManager(CONSTANTSTRINGRESOURCEID, typeof(ConstantString).Assembly);
    //    }

    //==========================================================//
    //for button text
    public static string Refresh => Localize("Refresh");

    public static string Edit => Localize("Edit");
    public static string Submit => Localize("Submit");
    public static string Delete => Localize("Delete");
    public static string Remove => Localize("Remove");
    public static string Add => Localize("Add");
    public static string Clone => Localize("Clone");
    public static string New => Localize("New");
    public static string Export => Localize("Export to Excel");
    public static string ExportPDF => Localize("Export to PDF");
    public static string Import => Localize("Import from Excel");
    public static string Actions => Localize("Actions");
    public static string Save => Localize("Save");
    public static string SaveAndNew => Localize("Save & New");
    public static string SaveChanges => Localize("Save Changes");
    public static string Cancel => Localize("Cancel");
    public static string Close => Localize("Close");
    public static string Search => Localize("Search");
    public static string Clear => Localize("Clear");
    public static string Reset => Localize("Reset");
    public static string Ok => Localize("OK");
    public static string Confirm => Localize("Confirm");
    public static string Yes => Localize("Yes");
    public static string No => Localize("No");
    public static string Next => Localize("Next");
    public static string Previous => Localize("Previous");
    public static string Upload => Localize("Upload");
    public static string Download => Localize("Download");
    public static string Uploading => Localize("Uploading...");
    public static string Downloading => Localize("Downloading...");
    public static string NoAllowed => Localize("No Allowed");
    public static string SigninWith => Localize("Sign in with {0}");
    public static string Logout => Localize("Logout");
    public static string Signin => Localize("Sign In");
    public static string Microsoft => Localize("Microsoft");
    public static string Facebook => Localize("Facebook");
    public static string Google => Localize("Google");

    //============================================================================//
    // for toast message
    public static string SaveSuccess => Localize("Save Success");

    public static string DeleteSuccess => Localize("Delete Success");
    public static string DeleteFail => Localize("Delete failed");
    public static string UpdateSuccess => Localize("AddOrUpdate Success");
    public static string UpdateFail => Localize("AddOrUpdate Failed");
    public static string CreateSuccess => Localize("Create Success");
    public static string LoginSuccess => Localize("Login Success");
    public static string LogoutSuccess => Localize("Logout Success");
    public static string LoginFail => Localize("Login Failed");
    public static string LogoutFail => Localize("Logout Failed");
    public static string ImportSuccess => Localize("Import Success");
    public static string ImportFail => Localize("Import fail");
    public static string ExportSuccess => Localize("Export Success");
    public static string ExportFail => Localize("Export fail");
    public static string UploadSuccess => Localize("Upload Success");

    //========================================================

    public static string Selected => Localize("Selected");
    public static string SelectedTotal => Localize("Selected Total");
    public static string AdvancedSearch => Localize("Advanced Search");
    public static string OrderBy => Localize("Order By");
    public static string CreateAnItem => Localize("Create a new {0}");
    public static string EditTheItem => Localize("Edit the {0}");
    public static string DeleteTheItem => Localize("Delete the {0}");
    public static string DeleteItems => Localize("Delete selected items: {0}");
    public static string DeleteConfirmation => Localize("Are you sure you want to delete this item: {0}?");

    public static string DeleteConfirmationWithId =>
        Localize("Are you sure you want to delete this item with Id: {0}?");

    public static string DeleteConfirmWithSelected =>
        Localize("Are you sure you want to delete the selected items: {0}?");

    public static string NoRecords => Localize("There are no records to view.");
    public static string Loading => Localize("Loading...");
    public static string Waiting => Localize("Wating...");
    public static string Processing => Localize("Processing...");
    public static string DeleteConfirmationTitle => Localize("Delete Confirmation");
    public static string LogoutConfirmationTitle => Localize("Logout Confirmation");

    public static string LogoutConfirmation =>
        Localize("You are attempting to log out of application. Do you really want to log out?");

    //Town relateds
    public static string TownAdmin => Localize("Town Admin");

    public static string Localize(string key)
        {
        // return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
        return key;
        }
    }
