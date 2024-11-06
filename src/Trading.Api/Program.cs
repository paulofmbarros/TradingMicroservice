using Microsoft.EntityFrameworkCore;
using Trading.Api.BackgroundServices;
using Trading.Application.Commands.ExecuteTrade;
using Trading.Domain.Interfaces;
using Trading.Infrastructure;
using Trading.Infrastructure.Producer;
using Trading.Infrastructure.Repositories;
using Trading.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23))));

builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

builder.Services.AddHostedService<OutboxBackgroundService>();
builder.Services.AddScoped<OutboxProcessor>(sp =>
{
    var outboxRepository = sp.GetRequiredService<IOutboxRepository>();
    var kafkaProducer = sp.GetRequiredService<IKafkaProducer>();
    var kafkaBroker = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092";
    return new OutboxProcessor(outboxRepository, kafkaProducer, kafkaBroker);
});


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ExecuteTradeCommandHandler>());

builder.Services.AddSingleton<IKafkaProducer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var kafkaBootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
    return new KafkaProducer(kafkaBootstrapServers);
});

builder.Services.AddControllers();


var app = builder.Build();

// Apply pending migrations at application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    dbContext.Database.Migrate();
}

// Apply pending migrations at application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

public partial class Program { }