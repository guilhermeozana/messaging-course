using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Data;
using Orders.Domain;
using Orders.Domain.StateMachine;
using Orders.Service;
using OrderSaga.StateMachine;
using OrdersApi.Infrastructure.Mappings;
using OrdersApi.Services;

namespace OrderSaga
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
                    
                    services.AddDbContext<OrderContext>(options =>
                    {
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                        options.EnableSensitiveDataLogging(true);

                    });
                    
                    services.AddScoped<IOrderRepository, OrderRepository>();
                    services.AddScoped<IOrderService, OrderService>();
                    services.AddAutoMapper(typeof(OrderProfileMapping));
                    
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();
                        
                        x.SetEntityFrameworkSagaRepositoryProvider(r =>
                        {
                            r.ExistingDbContext<OrderContext>();
                            r.UseSqlServer();
                        });

                        // By default, sagas are in-memory, but should be changed to a durable
                        // saga repository.
                        x.SetInMemorySagaRepositoryProvider();

                        x.AddSagaStateMachine<OrderStateMachine, OrderStateData>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ExistingDbContext<OrderContext>();
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                            });
                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);
                        //x.AddSagaStateMachines(entryAssembly);
                        //x.AddSagas(entryAssembly);
                        x.AddActivities(entryAssembly);

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseInMemoryScheduler();
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}