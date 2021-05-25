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
                option.ReportApiVersions = true;                                          // ��ѡ��ΪtrueʱAPI����֧�ֵİ汾��Ϣ
                option.AssumeDefaultVersionWhenUnspecified = true;                        // ���ṩ�汾ʱ��Ĭ��Ϊ1.0
                option.ApiVersionReader = new HeaderApiVersionReader("api-version");     //�汾��Ϣ�ŵ�header ,��д�ڲ�����·�ɵ�����£��汾��Ϣ�ŵ�response url ��
                option.DefaultApiVersion = new ApiVersion(1, 0);                         // ������δָ���汾ʱĬ��Ϊ1.0
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
                    t.Description = "�ӿ�˵���ĵ�";
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
