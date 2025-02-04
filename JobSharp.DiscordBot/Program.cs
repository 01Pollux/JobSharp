using Discord.Interactions;
using Discord.WebSocket;
using JobSharp.Core;
using JobSharp.Core.Config;
using JobSharp.Core.Services;
using JobSharp.DiscordBot;
using JobSharp.DiscordBot.Handlers;
using JobSharp.LinkedIn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddLogging();
builder.Services.AddJobSearchDescription(builder.Configuration);

builder.Services.AddHostedService<JobWatchDogService>();
builder.Services.AddSingleton<IJobScrapper, LinkedInScrapper>();
builder.Services.AddSingleton<IJobHandler, DispatchToDiscordChannelJobHandler>();

builder.Services
    .AddSingleton((_) =>
    {
        return new DiscordSocketConfig()
        {
            UseInteractionSnowflakeDate = false
        };
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton<InteractionService>(provider =>
    {
        return new(provider.GetRequiredService<DiscordSocketClient>());
    })
    .AddSingleton<InteractionHandler>()
    .AddHostedService<DiscordBotService>();

var app = builder.Build();
await app.RunAsync();

