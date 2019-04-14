using System;
using Achiever.Common;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Feed;
using Achiever.Core.Notifications;
using Achiever.Data;
using Achiever.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Swagger;

namespace Achiever.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true);

            Configuration = builder.Build();

            var elasticUri = Configuration["ElasticConfiguration:Uri"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                })
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authSettings = Configuration.GetSection("AuthSettings").Get<AuthSettings>();
            services.ConfigureAuth(authSettings.SecretKey);
            services.AddSingleton<ITokenService>(sp => new TokenService(authSettings.SecretKey));
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Achiever API", Version = "v1" });
            });

            var mongoSettings = Configuration.GetSection("MongoSettings").Get<MongoSettings>();
            services.AddSingleton(sp => new MongoContext(mongoSettings.Url));

            services.AddScoped<ICurrentUser, CurrentUser>();
            
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUserNotificationsRepository, UserNotificationsRepository>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IUserNotificationsFactory, UserNotificationsFactory>();

            services.AddSingleton<IAchievementRepository, AchievementRepository>();
            services.AddSingleton<IAcquiredAchievementRepository, AcquiredAchievementRepository>();

            services.AddSingleton<IFeedRepository, FeedRepository>();
            services.AddSingleton<ITestRepository, TestRepository>();
            services.AddSingleton<IGlobalFeedRepository, GlobalFeedRepository>();
            services.AddSingleton<ISocialInteractionsService, SocialInteractionsService>();
            services.AddSingleton<IFilesRepository, FilesRepository>();
            
            services.AddSingleton<IFeedService, FeedService>();
            services.AddSingleton<IAchievementService, AchievementService>();
            services.AddSingleton<IImageProcessingService, ImageProcessingService>();

            services.AddSingleton<INotificationSender, NotificationSender>();

            services.AddSingleton<IFileService>(sp =>
            {
                var webRoot = sp.GetService<IHostingEnvironment>().WebRootPath;
                var filesRepository = sp.GetService<IFilesRepository>();
                return new FileService(webRoot, filesRepository);
            });

            services.AddSingleton<IAchievementCategoryRepository, AchievementCategoryRepository>();
            services.AddSingleton<IAchievementCategoryService, AchievementCategoryService>();

            services.AddSingleton<IAcquiredAchievementService, AcquiredAchievementService>();

            services.AddSingleton<ISearchService, SearchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Achiever API V1");
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });
            
            app.UseMvc();
        }
    }
}
