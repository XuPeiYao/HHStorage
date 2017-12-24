using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EzCoreKit.AspNetCore;
using EzCoreKit.Cryptography;
using EzCoreKit.MIME;
using HHStorage.Exceptions;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                        Description = "請輸入Bearer類型的JWT在此欄位",
                        Name = "Authorization",
                        Type = "apiKey",
                    });
                void IncludeXmlComment(string filename) {
                    c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, filename));
                }
                IncludeXmlComment("HHStorage.xml");
                IncludeXmlComment("HHStorage.Models.API.xml");
                IncludeXmlComment("HHStorage.Exceptions.xml");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            //錯誤處理
            Action<IApplicationBuilder> errorHandler = (builder) => {
                builder.Run(handler => {
                    return Task.Run(() => {
                        //取得狀態碼
                        int code = handler.Response.StatusCode;

                        //檢查是否存在指定的對應
                        if (code == 401) {
                            handler.Response.ContentType = DeclareMIME.JavaScript_Object_Notation_JSON;

                            //寫出錯誤頁面內容
                            var serializerSettings = new JsonSerializerSettings();
                            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                            byte[] Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new AuthorizeException(), serializerSettings));

                            handler.Response.Body.WriteAsync(Content, 0, Content.Length);
                            return;
                        }
                    });
                });
            };

            //狀態對應
            app.UseStatusCodePages(errorHandler);

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
