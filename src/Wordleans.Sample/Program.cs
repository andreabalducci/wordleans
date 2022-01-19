using Orleans;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Streams;
using Wordleans.Api.Services;
using Wordleans.Kernel.Stats;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IClock>(new SystemClock());
builder.Host.UseOrleans(siloBuilder => siloBuilder
    .UseDashboard(config =>
    {
        // config.Port = 8000;
        config.HostSelf = true;
    })
    .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
    .UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000)
    .UseInMemoryReminderService()
    .AddMemoryStreams<DefaultMemoryMessageBodySerializer>(StatsDefaults.StatsProvider,
        config => { config.ConfigureStreamPubSub(StreamPubSubType.ImplicitOnly); })
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();