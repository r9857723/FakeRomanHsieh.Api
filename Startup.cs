using System;
using System.Linq;
using System.Text;
using FakeRomanHsieh.API.Database;
using FakeRomanHsieh.API.Models;
using FakeRomanHsieh.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace FakeRomanHsieh.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 注入身份驗證
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // 注入JWT驗證
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretBtye = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // 驗證發佈者
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        // 驗證token持有者
                        ValidateAudience = true,
                        ValidAudience = Configuration["Authentication:Audience"],
                        // 驗證是否過期
                        ValidateLifetime = true,
                        // 取得私鑰
                        IssuerSigningKey = new SymmetricSecurityKey(secretBtye)

                    };
                });

            services.AddControllers(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
            })
                .AddNewtonsoftJson(setupAction => {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                //驗證失敗回傳422
                .ConfigureApiBehaviorOptions(setupAction => {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "無所謂",
                            Title = "數據驗證失敗",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "請看詳細內容",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("TraceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                }); 

            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            services.AddDbContext<AppDbContext>(options=> {
                //MsSql
                options.UseSqlServer(Configuration["DbContext:ConnectionString"]);
                //MySql
                //options.UseMySql(Configuration["DbContext:MySQLConnectionString"], ServerVersion.AutoDetect(Configuration["DbContext:MySQLConnectionString"]));
            });
            //掃描profile文件
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpClient();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.Configure<MvcOptions>(config =>
            {
                var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.Roman.hateoas+json");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 路由 （你在哪
            app.UseRouting();
            // 用戶 （你是誰
            app.UseAuthentication();
            // 權限 （你可以幹嘛
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
            });
        }
    }
}
