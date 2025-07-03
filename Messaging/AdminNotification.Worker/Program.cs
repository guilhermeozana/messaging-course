using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Orders.Domain;

namespace AdminNotification.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();
                        
                        var entryAssembly = Assembly.GetEntryAssembly();
                        x.AddConsumers(entryAssembly);
                        
                        x.AddEntityFrameworkOutbox<OrderContext>(o =>
                        {
                            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                            //o.QueryDelay = TimeSpan.FromSeconds(5);
                            o.UseSqlServer();
                            //o.DisableInboxCleanupService();
                            //o.UseBusOutbox(x => x.DisableDeliveryService());
                            o.UseBusOutbox();
                        });
                        
                        x.AddConfigureEndpointsCallback((context, name, cfg) =>
                        {
                            cfg.UseEntityFrameworkOutbox<OrderContext>(context);
                        });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ReceiveEndpoint("order-created", e =>
                            {
                                //e.UseEntityFrameworkOutbox<OrderContext>(context);
                                e.ConfigureConsumer<OrderCreatedNotification>(context);
                            });
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}