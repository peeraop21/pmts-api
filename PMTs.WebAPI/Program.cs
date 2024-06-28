using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using PMTs.DataAccess;
using PMTs.DataAccess.Email;
using PMTs.DataAccess.Email.Interfaces;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.Redis.Interfaces;
using PMTs.DataAccess.Redis;
using PMTs.DataAccess.Tracing;
using PMTs.WebAPI.AutoMapper;
using Prometheus;
using Rotativa.AspNetCore;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore;
using Autofac.Core;

//namespace PMTs.WebAPI
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateWebHostBuilder(args).Build().Run();
//        }

//        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>();

//        // .UseKestrel(options =>
//        //{
//        //    options.Limits.MaxRequestBodySize = 52428800; //50MB
//        //    options.Limits.MaxResponseBufferSize = 52428800;
//        //});
//        //}
//    }// Fix Bug
//}// bo fixed

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenTelemetryParameters>(builder.Configuration.GetSection("OpenTelemetry"));
var openTelemetryParameters = builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtIssuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JwtAudience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSecretKey")))
    };
});

builder.Services.AddDbContext<PMTsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PMTsConnect")));

builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
builder.Services.AddControllers().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; }).AddDataAnnotationsLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en"),
        new CultureInfo("th"),
        new CultureInfo("vi"),
    };
    opts.DefaultRequestCulture = new RequestCulture("en-GB");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
});
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PMTs API",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAutoMapper(typeof(AutoMapping));
builder.Services.AddControllers();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddOpenTelemetryMetrics(builder.Configuration);
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

var app = builder.Build();

//app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.UseRouting();
app.UseMvc();
app.UseAuthorization();

app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); // Expose metrics for Prometheus
    endpoints.MapRazorPages();
});

app.MapControllers();

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.UseStaticFiles();
app.UseDefaultFiles();

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);

app.Run();