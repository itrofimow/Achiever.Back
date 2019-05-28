using System;
using Horarium;
using Horarium.Interfaces;
using Horarium.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Achiever.ImageProcessor.Horarium
{
    public static class HorariumRegistrationExtensions
    {
        public static void StartHorarium(this IServiceProvider serviceProvider)
        {
            var settings = new HorariumSettings
            {
                JobFactory = new JobFactory(serviceProvider)
            };
            
            var horarium = new HorariumServer(MongoRepositoryFactory.Create(
                "mongodb://localhost:27017/achiever_horarium_db"), settings);

            horarium.CreateRecurrent<ImageCompressionJob>(Cron.SecondInterval(10)).Schedule().Wait();
        }
    }

    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public object CreateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        public IDisposable BeginScope()
        {
            return _serviceProvider.CreateScope();
        }
    }
}