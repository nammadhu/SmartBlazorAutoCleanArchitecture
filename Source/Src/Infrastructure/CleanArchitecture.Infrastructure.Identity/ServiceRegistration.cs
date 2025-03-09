using CleanArchitecture.Application.Interfaces.UserInterfaces;
using CleanArchitecture.Domain.AspNetIdentity;
using CleanArchitecture.Infrastructure.Identity.Contexts;
using CleanArchitecture.Infrastructure.Identity.Services;
using CleanArchitecture.Infrastructure.Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CleanArchitecture.Infrastructure.Identity;

public static class ServiceRegistration
    {
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabase)
        {

        if (useInMemoryDatabase)
            {
            //use AddDbContextFactory instead of AddDbContext to avoid second operation query issue
            services.AddDbContextFactory<IdentityContext>(options =>
                options.UseInMemoryDatabase(nameof(IdentityContext)));
            }
        else
            {
            //services.AddDbContext<IdentityContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            //use AddDbContextFactory instead of AddDbContext to avoid second operation query issue
            services.AddDbContextFactory<IdentityContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")
                , sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(3),
                        errorNumbersToAdd: null);
                }));
            }

        services.AddTransient<IGetUserServices, GetUserServices>();
        services.AddTransient<IAccountServices, AccountServices>();

        var identitySettings = configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();

        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        services.AddSingleton(jwtSettings);

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = false;

            options.Password.RequireDigit = identitySettings.PasswordRequireDigit;
            options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.PasswordRequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.PasswordRequireUppercase;
            options.Password.RequireLowercase = identitySettings.PasswordRequireLowercase;

        }).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
        //AddIdentity itself adds AddAuthentication() default features, if more like google required then below is required


        services.AddAuthentication()
            .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        })
            .AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/Account/Login";
                cookieOptions.LogoutPath = "/Account/Logout";
                cookieOptions.AccessDeniedPath = "/Account/AccessDenied";
                cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(14); // Example expiration setting
                cookieOptions.SlidingExpiration = true; // Renew the cookie if the user is active
            });
        /*
         // Microsoft Authentication
         .AddMicrosoftAccount(microsoftOptions =>
         {
             microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
             microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
         })
         // Facebook Authentication
         .AddFacebook(facebookOptions =>
         {
             facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
             facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
         })
         // LinkedIn Authentication
         .AddLinkedIn(linkedInOptions =>
         {
             linkedInOptions.ClientId = configuration["Authentication:LinkedIn:ClientId"];
             linkedInOptions.ClientSecret = configuration["Authentication:LinkedIn:ClientSecret"];
         })
         // Instagram Authentication
         .AddInstagram(instagramOptions =>
         {
             instagramOptions.ClientId = configuration["Authentication:Instagram:ClientId"];
             instagramOptions.ClientSecret = configuration["Authentication:Instagram:ClientSecret"];
         })
         */


        /* Below are for API responding, not needed for Blazor integrated login
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
                o.Events = new JwtBearerEvents()
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(BaseResult.Failure(new Error(ErrorCode.AccessDenied, "You are not Authorized")));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsJsonAsync(BaseResult.Failure(new Error(ErrorCode.AccessDenied, "You are not authorized to access this resource")));
                    },
                    OnTokenValidated = async context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims.Any() is not true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity?.FindFirst("AspNet.Identity.SecurityStamp");
                        if (securityStamp is null)
                            context.Fail("This token has no security stamp");

                        var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                        var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                        if (validatedUser is null)
                            context.Fail("Token security stamp is not valid.");
                    },

                };
            });
        */
        return services;
        }
    }
