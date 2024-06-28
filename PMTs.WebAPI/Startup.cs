using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PMTs.DataAccess;
using PMTs.WebAPI.AutoMapper;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PMTs.WebAPI
{
    public class Startup()
    {
        public IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = Configuration.GetValue<string>("JwtIssuer"),
                      ValidAudience = Configuration.GetValue<string>("JwtAudience"),
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSecretKey")))
                  };
              });

            //services.AddDbContext<PMTsDbContext>(options => options.UseSqlServer(Configuration.GetValue<string>("ConnectionString"), b => b.UseRowNumberForPaging()));
            services.AddDbContext<PMTsDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PMTsConnect")));


            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddMvc()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
            opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("th"),
                    new CultureInfo("vi"),
                    //new CultureInfo("en-GB"),
                    //new CultureInfo("en-US"),
                };

                opts.DefaultRequestCulture = new RequestCulture("en-GB");
                // opts.DefaultRequestCulture = new RequestCulture("th");
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });

            //services.AddMvc();
            //services.AddMvc()
            //    .AddJsonOptions(
            //        options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //    );

            services.AddMvc()
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info { Title = "PMTs API", Version = "v1" });
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PMTs API",
                });
                //c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                //{ "Bearer", Enumerable.Empty<string>() },
                //});
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

            services.AddAutoMapper(typeof(AutoMapping));

            services.AddMvc(options => options.EnableEndpointRouting = false);
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<MasterUser, UserDTO>();
            //});

            #region [temp old config]
            //===========old config
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //// Register the Swagger generator, defining 1 or more Swagger documents
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "PMTs API", Version = "v1" });
            //});

            //services.AddDbContext<PMTsDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PMTsConnect")));
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseAuthentication();
            app.UseRouting();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "PMTs API V1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

            app.UseStaticFiles();
            app.UseDefaultFiles();
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #region[old config]
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            ////app.UseHttpsRedirection();
            ////// app.UseMvc();
            ////app.UseMvc(routes =>
            ////{
            ////    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            ////});


            //// Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            //// specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("../swagger/v1/swagger.json", "PMTs API V1");
            //});

            //app.UseHttpsRedirection();
            //app.UseMvc();
            #endregion
        }
    }
}
