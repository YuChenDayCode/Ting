using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Ting.Util;

namespace Ting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IApiVersionDescriptionProvider provider;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;                                          // 可选，为true时API返回支持的版本信息
                option.AssumeDefaultVersionWhenUnspecified = true;                        // 不提供版本时，默认为1.0
                option.ApiVersionReader = new HeaderApiVersionReader("api-version");     //版本信息放到header ,不写在不配置路由的情况下，版本信息放到response url 中
                option.DefaultApiVersion = new ApiVersion(1, 0);                         // 请求中未指定版本时默认为1.0
            }).AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'V";
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            this.provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (ApiVersionDescription desc in provider.ApiVersionDescriptions)
            {
                services.AddSwaggerDocument(t =>
                {
                    t.Title = "Ting Api";
                    t.Version = desc.GroupName;
                    t.DocumentName = desc.GroupName;
                    t.Description = "接口说明文档";
                    t.ApiGroupNames = new string[] { desc.GroupName };
                    t.UseControllerSummaryAsTagDescription = true;
                });
            }
            services.AddControllers();
            services.AddHttpClient<TingClient>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
