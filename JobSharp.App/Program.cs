using JobSharp.Core;
using JobSharp.Core.Handlers;
using JobSharp.Core.Payload;
using JobSharp.Core.Services;
using JobSharp.LinkedIn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddLogging();

builder.Services.AddSingleton(new JobSearchDescription(SearchTerm: "Software Engineer", ExperienceLevel: JobExperienceLevel.Intern));

builder.Services.AddSingleton<IJobScrapper, LinkedInScrapper>();

builder.Services.AddSingleton<IJobHandler, PrintToConsoleJobHandler>();

builder.Services.AddHostedService<JobWatchDogService>();

using var app = builder.Build();
await app.RunAsync();
