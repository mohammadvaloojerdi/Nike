using System;
using System.Collections.Generic;
using System.Diagnostics;
using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Framework.Domain;
using Nike.Mediator.Handlers;
using Nike.Redis.Microsoft.DependencyInjection;
using Nike.SampleProducer.Model;

namespace Nike.SampleProducer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Test(host.Services);
            host.Run();
        }

        private static void Test(IServiceProvider serviceProvider)
        {
            var dispatcher = serviceProvider.GetRequiredService<IEventBusDispatcher>();

            var lst = new List<BenchmarkModelIntegrationEvent>();
            var size = 10000;
            for (int i = 0; i < size; i++)
            {
                lst.Add(new BenchmarkModelIntegrationEvent($"msg_{i}", "sample desc", i));
            }

            var sw = Stopwatch.StartNew();
            foreach (var model in lst)
            {
                dispatcher.Publish(model);
            }

            sw.Stop();

            Console.WriteLine(
                $"Publishing {size} msgs to Kafka in {sw.Elapsed.TotalMilliseconds}. MEANS: {sw.Elapsed.TotalMilliseconds / size}");
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices(Configuration);
        }

        private static void Configuration(HostBuilderContext hostContext, IServiceCollection services)
        {
            ConfigureRedis(hostContext, services);
            ConfigureKafka(hostContext, services);
            ConfigureMicroBus(services);
            services.AddSingleton<IClock, SystemClock>();
        }


        #region PrivateMethods

        private static void ConfigureRedis(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddRedis(hostContext.Configuration.GetSection("Cache").Get<RedisConfig>());
        }

        private static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
        {
            var busConfig = hostContext.Configuration.GetSection("EventBus").Get<EventBusConfig>();
            services.AddKafkaProducer(busConfig.ConnectionString);
        }

        private static void ConfigureMicroBus(IServiceCollection services)
        {
            services.RegisterMicroBus(new BusBuilder()
                .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
                .RegisterHandlers(typeof(Program).Assembly));
        }

        #endregion
    }
}