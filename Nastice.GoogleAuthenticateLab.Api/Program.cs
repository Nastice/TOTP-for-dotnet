using System.Configuration;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DAOs;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Data.Repositories;
using Nastice.GoogleAuthenticateLab.Middlewares;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Services.Libraries;
using Nastice.GoogleAuthenticateLab.Services.Services;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Filters;
using Nastice.GoogleAuthenticateLab.Shared.Models.Options;
using Nastice.GoogleAuthenticateLab.Shared.ValidationManagers;
using Nastice.GoogleAuthenticateLab.Shared.Validations;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .Destructure.ToMaximumDepth(2);

Log.Logger = logger.CreateLogger();

try
{
    Log.Information("Initializing and starting web host");

    #region Initialize SeriLog

    builder.Services.AddSerilog();

    #endregion

    #region Register Configuration

    var jwtOptionsSection = builder.Configuration.GetSection("JwtOptions");

    builder.Services.Configure<JwtOptions>(jwtOptionsSection);
    builder.Services.Configure<AesOptions>(builder.Configuration.GetSection("AesOptions"));

    #endregion

    #region Register Cors

    builder.Services.AddCors(options => {
        options.AddPolicy("DefaultCorsPolicy",
            policy => {
                var allowOriginSection = builder.Configuration.GetSection("CorsPolicy:AllowOrigins");
                var allowMethodsSection = builder.Configuration.GetSection("CorsPolicy:AllowMethods");
                var allowOrigin = allowOriginSection.Get<string[]>() ?? [];
                policy.WithOrigins(allowOrigin);

                var allowMethod = allowMethodsSection.Get<string[]>() ?? [];
                policy.WithMethods(allowMethod);

                policy.AllowAnyHeader();
                policy.AllowCredentials();
            });
    });

    #endregion

    #region Register Authorize & Authenticate

    builder.Services.AddAuthorization();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            var jwtOptions = jwtOptionsSection.Get<JwtOptions>();
            if (string.IsNullOrEmpty(jwtOptions?.Secret))
            {
                throw new ConfigurationErrorsException("Jwt Secret 尚未設定，請先設定 JwtOptions");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                IssuerSigningKeys = [key]
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context => {
                    var token = context.Request.Cookies["access_token"];
                    context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("default",
            policy => {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });

    #endregion

    #region Register Controllers

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddControllers(options => {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<TraceLogFilter>();
        })
        .AddJsonOptions(options => {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.MaxDepth = 5;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            options.JsonSerializerOptions.WriteIndented = true;
        });

    #endregion

    #region Register Services

    builder.Services.AddProxiedScoped<IAuthService, AuthService>();

    #endregion

    #region Register Libraries

    builder.Services.AddProxiedScoped<JwtTokenLibrary>()
        .AddProxiedKeyedScoped<ISecurityLibrary, AesSecurityLibrary>(nameof(AesSecurityLibrary))
        .AddProxiedScoped<AesSecurityLibrary>();

    #endregion

    #region Register Repositories

    builder.Services.AddProxiedScoped<IRepositoryBase<User>, UserRepository>();

    #endregion

    #region Register Db Context

    builder.Services.AddDbContext<DbContext, MssqlContext>(options => {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        options.UseSqlServer(connectionString,
            msSqlOptions => {
                var assembly = typeof(MssqlContext).Assembly;
                var assemblyName = assembly.GetName()
                    .Name;

                msSqlOptions.MigrationsAssembly(assemblyName);
            });

        options.EnableSensitiveDataLogging();
        options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
    });

    #endregion

    #region Register Cache Service

    builder.Services.AddStackExchangeRedisCache(options => {
        options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
        options.InstanceName = "Nastice:GoogleAuthenticateLab:";
    });
    builder.Services.AddDistributedMemoryCache();

    #endregion

    #region Register Fluent Validations

    ValidatorOptions.Global.LanguageManager = new ZhLanguageManager();

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>(ServiceLifetime.Transient);

    #endregion

    #region Build Web Application and Start

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseForwardedHeaders(new()
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.UseCors("DefaultCorsPolicy");

    app.UseMiddleware<CsrfValidationMiddleware>();

    await app.RunAsync();

    #endregion
    return 0;
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
    return 1;
}
