using System.Reflection;
using Contracts.Events;
using Contracts.Filters;
using Contracts.Infrastructure;
using Contracts.Models;
using Contracts.Response;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Domain;
using Orders.Service;
using OrdersApi.Consumers;
using OrdersApi.Infrastructure.Mappings;
using OrdersApi.Service.Clients;
using OrdersApi.Services;

namespace OrdersApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddAutoMapper(typeof(OrderProfileMapping));
            builder.Services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

            });

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<Tenant>();

            builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>();

            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                
                x.AddEntityFrameworkOutbox<OrderContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox(x => x.DisableDeliveryService());
                    //o.UseBusOutbox();
                });
                
                //x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("hellos", false));
                
                //x.AddConsumer<OrderCreatedConsumer, OrderCreatedConsumerDefinition>();
                //x.AddConsumer<OrderCreatedConsumer>();
                //x.AddConsumer<VerifyOrderConsumer>();
                //register fault consumers
                //x.AddConsumer<OrderCreatedFaultConsumer>();
                //x.AddConsumer<AllFaultsConsumer>();
                
                //x.AddRequestClient<VerifyOrder>();
                //x.AddConsumer(typeof(OrderCreatedConsumer), typeof(OrderCreatedConsumerDefinition));
                
                //x.AddConsumer(typeof(OrderCreatedConsumer));

                // var entryAssembly = Assembly.GetEntryAssembly();
                // x.AddConsumers(entryAssembly);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r =>
                    {
                        r.Immediate(2);
                    });
                    // cfg.Host("rabbitmq://localhost", "/", h =>
                    // {
                    //     h.Username("guest");
                    //     h.Password("guest");
                    // });

                    cfg.SendTopology.ErrorQueueNameFormatter = new MyCoolErrorQueueNameFormatter();
                    
                    cfg.UseSendFilter(typeof(TenantSendFilter<>), context);
                    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context);
                    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context, x => x.Include(typeof(Email)));
                    //cfg.UseConsumeFilter(typeof(TenantPublishFilter<>), context);
                    
                    cfg.UsePublishFilter<TenantPublishEmailFilter>(context);
                    //cfg.UseFilter(new TenantConsumeFilter<OrderCreated>());
                    cfg.UseFilter(new MyCoolFilter());
                    
                    // cfg.ReceiveEndpoint("order-created", e =>
                    // {
                    //     e.UseFilter(new TenantConsumeFilter<OrderCreated>());
                    //     e.UseMessageRetry(x => x.Interval(2, 500));
                    //     e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    // });
                    
                    cfg.ConfigureEndpoints(context);
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
