using Amazon;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using OrderProcess.Adapters;

namespace OrderProcess.Core;

public static class ConfigureServices
{
    public static void Configure(this IServiceCollection services)
    {
        services.AddTransient<IAmazonSQS, AmazonSQSClient>(_ => new AmazonSQSClient(RegionEndpoint.EUWest2));
        services.AddTransient<OrderRepository>();
    }
}