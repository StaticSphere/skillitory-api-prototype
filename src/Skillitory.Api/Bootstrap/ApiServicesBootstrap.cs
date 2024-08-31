using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Asp.Versioning;
using FastEndpoints;
using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Skillitory.Api.DataStore;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Bootstrap;

[ExcludeFromCodeCoverage]
public static partial class ApiServicesBootstrap
{
    [GeneratedRegex("I(?:.+)DataService", RegexOptions.Compiled)]
    private static partial Regex InterfacePatternMatcher();

    private static readonly Regex InterfacePattern = InterfacePatternMatcher();

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration,
        string corsPolicy)
    {
        var securitySettings = configuration.GetSection("Security").Get<SecurityConfiguration>()!;
        services.Configure<SecurityConfiguration>(options => configuration.GetSection("Security").Bind(options));
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = securitySettings.Password.MinimumLength;
            options.Password.RequireNonAlphanumeric = securitySettings.Password.RequireSymbols;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = securitySettings.Password.RequiredUniqueCharacters;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.Lockout.TimeSpanMinutes);
            options.Lockout.MaxFailedAccessAttempts = securitySettings.Lockout.MaxFailedAttempts;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        });

        services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = securitySettings.Jwt.ValidIssuer,
                    ValidAudience = securitySettings.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.Jwt.Secret)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                // x.Events = new JwtBearerEvents
                // {
                //     OnMessageReceived = context =>
                //     {
                //         if (!context.Request.Cookies.TryGetValue(securitySettings.AccessCookieName, out var authCookie))
                //             return Task.CompletedTask;
                //
                //         try
                //         {
                //             var tokenData = JsonSerializer.Deserialize<TokenData>(authCookie);
                //             context.Token = tokenData!.AccessToken;
                //         }
                //         catch (JsonException)
                //         {
                //             // Ignore; just assume that the caller is not authenticated
                //         }
                //
                //         return Task.CompletedTask;
                //     }
                // };
            });

        services.AddAuthorization();

        services
            .AddDbContext<SkillitoryDbContext>()
            .AddScoped<ISkillitoryDbContext, SkillitoryDbContext>();

        services
            .AddIdentityCore<AuthUser>()
            .AddRoles<AuthRole>()
            .AddEntityFrameworkStores<SkillitoryDbContext>()
            .AddDefaultTokenProviders();

        var types = Assembly.GetExecutingAssembly().GetTypes();
        (from c in types
                where c.IsInterface && InterfacePattern.IsMatch(c.Name)
                from i in types
                where i.GetInterface(c.Name) is not null
                select new { Contract = c, Implementation = i }).ToList()
            .ForEach(x => services.AddScoped(x.Contract, x.Implementation));

        var smtpConfiguration = configuration.GetSection("Smtp").Get<SmtpConfiguration>();
        services
            .AddFluentEmail(smtpConfiguration!.DefaultSender)
            .AddLiquidRenderer(options =>
            {
                options.FileProvider = new EmbeddedFileProvider(
                    Assembly.GetExecutingAssembly(),
                    "Skillitory.Api.Resources.EmailTemplates");
            });

        services.AddSingleton<ISender>(_ => new MailKitSender(new SmtpClientOptions
        {
            Server = smtpConfiguration.Server,
            Port = smtpConfiguration.Port,
            User = smtpConfiguration.UserName,
            Password = smtpConfiguration.Password,
            UseSsl = smtpConfiguration.UseSsl,
            UsePickupDirectory = smtpConfiguration.UsePickupDirectory,
            MailPickupDirectory = smtpConfiguration.MailPickupDirectory
        }));

        // General API services
        services.AddCors(options =>
        {
            options.AddPolicy(
                corsPolicy,
                policyBuilder =>
                {
                    policyBuilder.WithOrigins(configuration["WebAppUrl"]!.Trim('/', '\\'))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersion.Default;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddHttpContextAccessor();
        services.AddHealthChecks()
            .AddDbContextCheck<SkillitoryDbContext>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IPrincipalService, PrincipalService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddScoped<IAuditService, AuditService>();
        //services.AddSingleton<IFileService, DiskFileService>();
        //services.AddSingleton<IStoredImageService, StoredImageService>();
        services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));

        services.AddFastEndpoints();

        return services;
    }
}
