using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocPlus.Data;
using SocPlus.Models;
using SocPlus.Services;
using System.Security.Claims;

namespace SocPlus.Utilities; 
public static class AppConfig {
    public static void CheckVariables(this IConfiguration conf) {
        List<string> variables = [
            "MailCreds:Username",
            "MailCreds:Password",
            "JwtConfig:Secret",
            "JwtConfig:Issuer",
            "JwtConfig:Audience",
            "JwtConfig:AccessHours",
            //"AdminPasswordHash",
            "SqlServerConnectionString",
        ];
        List<string?> values = [];
        foreach (string var in variables) {
            values.Add(conf.GetValue<string?>(var));
        }
        if (values.Any(v => v is null)) {
            Console.WriteLine($"Environment Variables: {variables.ToCSVColumn()}" +
                $"  need to be set in order for this app to function.\n" +
                $"You are missing: {variables.Where((_, i) => values[i] is null).ToCSVColumn()}");
            Environment.Exit(1);
        }
    }

    public static IServiceCollection ConfigureApp(this IServiceCollection services, IConfiguration conf) {
        services.AddCors(o => {
            o.AddDefaultPolicy(pol => {
                pol.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
            });
        });
        services.AddDbContext<SocPlusDbContext>(ob => {
            ob.UseSqlServer(conf["SqlServerConnectionString"]);
        });
        var jwtConfig = conf.GetSection("JwtConfig").Get<JwtConfig>()!;
        var mailCreds =  conf.GetSection("MailCreds").Get<MailCreds>()!;
        services.AddDependencies(mailCreds, jwtConfig);
        services.AddJwtAuth(jwtConfig);
        services.AddHttpClient();
        services.AddSwaggerGen(setup => {
            var jwtSecurityScheme = new OpenApiSecurityScheme {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Access Token",

                Reference = new OpenApiReference {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
         });
        return services;
    }
    public static IServiceCollection AddDependencies(this IServiceCollection services, MailCreds mailCreds, JwtConfig jwtConfig) {
        services.AddScoped<AuthService>();
        services.AddScoped<MailService>();
        services.AddScoped<PostService>();
        services.AddScoped<MailCreds>(_ => mailCreds);
        services.AddScoped<JwtConfig>(_ => jwtConfig);
        return services;
    }
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, JwtConfig jwtConfig) {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
            o.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromHexString(jwtConfig.Secret))
            };
            o.Events = new JwtBearerEvents {
                OnTokenValidated = async context => {
                    var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId)) {
                        context.Fail("Invalid token: Missing userId ('sub' claim)");
                        return;
                    }
                    var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
                    var exists = await authService.UserExistsAsync(userId);
                    if (!exists) {
                        context.Fail("Invalid token: This token doesn't belong to a registered user");
                    }
                }
            };
        });
        services.AddAuthorization();
        return services;

    }
}
