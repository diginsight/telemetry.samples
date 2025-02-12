using Diginsight.Diagnostics;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace SampleCoconaApp;

internal sealed class Executor : IDisposable
{
    private readonly ILogger logger;
    private readonly CosmosClient cosmosClient;
    private readonly Container container;
    private readonly string? file;
    private readonly bool whatIf;
    private readonly int? top;

    public Executor(ILogger<Executor> logger)
    {
        this.logger = logger;

        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger);

        //cosmosClient = new CosmosClient(connectionString);
        //container = cosmosClient.GetContainer("elcommon", "userplant");
        //this.file = file;
        //this.whatIf = whatIf;
        //this.top = top;
    }

    public void Dispose()
    {
        cosmosClient?.Dispose();
    }

    public async Task ExecuteAsync(
        [FromService] CoconaAppContext appContext,
        [Option('c')] string connectionString,
        [Option('q')] string query,
        [Option('d')] string database,
        [Option('t')] string collection,
        [Option('f')] string? file,
        [Option] int top = 100,
        [Option] int skip = 0
    )
    {
        try
        {
            string accountEndpoint = connectionString.Split(';')
                .Select(static x => x.Split('=', 2)).First(static x => x[0].Equals("AccountEndpoint", StringComparison.OrdinalIgnoreCase))[1];

            using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { accountEndpoint, query, file, top, skip });

            var cosmosClient = new CosmosClient(connectionString);
            var container = cosmosClient.GetContainer(database, collection ); 

            var topClause = top > 0 ? $" OFFSET {skip} LIMIT {top}" : string.Empty;
            string modifiedQuery = $"{query}{topClause}";
            logger.LogDebug("modifiedQuery: {modifiedQuery}", modifiedQuery);

            var requestOptions = new QueryRequestOptions { MaxItemCount = top, QueryTextMode = QueryTextMode.None }; 
            var iterator = container.GetItemQueryStreamIterator(modifiedQuery, requestOptions: requestOptions);

            StreamWriter? streamWriter = null;
            if (file is not null)
            {
                logger.LogInformation("Writing results to {File}", file);
                streamWriter = new StreamWriter(file);
            }

            using (streamWriter)
            {
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    if (!response.IsSuccessStatusCode) { throw new Exception(response.ErrorMessage); }
                    
                    response.Content.Position = 0;
                    var content = await new System.IO.StreamReader(response.Content).ReadToEndAsync();
                    logger.LogDebug("content: {content}", content);
                    if (streamWriter is not null)
                    {
                        await streamWriter.WriteAsync(content);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Uncaught exception");
        }
    }
}
