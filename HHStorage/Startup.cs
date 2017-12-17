using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EzCoreKit.AspNetCore;
using EzCoreKit.Cryptography;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace HHStorage {
    public class Startup {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<HHStorageContext>(options => {
                options.UseSqlServer(Configuration["ConnectionString"]);
            });

            services.AddEzJwtBearerWithDefaultSchema(
                new SymmetricSecurityKey(Configuration["SecretKey"].ToHash<MD5>()),
                "AnonChat");

            services.AddMvc();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new Info {
                        Title = "HHStorage API",
                        Version = "v1"
                    }
                 );
                c.AddSecurityDefinition(
                    "Bearer",
                    new ApiKeyScheme() {
                        In = "header",
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                void IncludeXmlComment(string filename) {
                    c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, filename));

                }
                IncludeXmlComment("HHStorage.xml");
                IncludeXmlComment("HHStorage.Models.API.xml");

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger(c => {
                c.RouteTemplate = "swagger/api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("api-docs/v1/swagger.json", "HHStorage API");
            });
        }
    }
}
