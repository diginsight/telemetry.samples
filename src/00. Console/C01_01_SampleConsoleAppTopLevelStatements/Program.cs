﻿using Diginsight;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleConsoleAppTopLevelStatementsTopLevelStatements;

var app = Host.CreateDefaultBuilder(args)
              //.ConfigureAppConfiguration(static (configuration) => { })
              .ConfigureServices((context, services) =>
              {
                  IConfiguration configuration = context.Configuration;
                  IHostEnvironment hostEnvironment = context.HostingEnvironment;

                  services.AddObservability(configuration, hostEnvironment);

                  services.AddSingleton<Executor>();
              });

app.UseDiginsightServiceProvider(true);
var host = app.Build();

var executor = host.Services.GetRequiredService<Executor>();

await executor.ExecuteAsync();

