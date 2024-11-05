using Microsoft.EntityFrameworkCore;
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ExecuteTradeCommandHandler>());

builder.Services.AddSingleton<IKafkaProducer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var kafkaBootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
    return new KafkaProducer(kafkaBootstrapServers);
});

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();