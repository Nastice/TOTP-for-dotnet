using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DAOs;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Data.Repositories;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Services.Services;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Filters;
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

    #region Register Controllers

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services
           .AddControllers(options => {
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

    #region Register Repositories

    builder.Services.AddProxiedScoped<IRepositoryBase<User>, UserRepository>();

    #endregion

    #region Db Context

    builder.Services.AddDbContext<DbContext, MssqlContext>(options => {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        options.UseSqlServer(connectionString,
                             msSqlOptions => {
                                 var assembly = typeof(MssqlContext).Assembly;
                                 var assemblyName = assembly.GetName().Name;

                                 msSqlOptions.MigrationsAssembly(assemblyName);
                             }
        );

        options.EnableSensitiveDataLogging();
        options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
    });

    #endregion

    #region Fluent Validations

    ValidatorOptions.Global.LanguageManager = new ZhLanguageManager();

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>(ServiceLifetime.Transient);

    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.MapControllers();

    await app.RunAsync();
    return 0;
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
    return 1;
}
