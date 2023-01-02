using System.Text.Json;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using OrderProcess.Core;
using OrderProcess.Core.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OrderProcess.ReceiveOrder;

public class Function
{
    private readonly IServiceCollection _serviceCollection;

    public Function()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.Configure();
    }

    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<Order> FunctionHandler(Order order, ILambdaContext context)
    {
        await using var serviceProvider = _serviceCollection.BuildServiceProvider();
        var client = serviceProvider.GetService<IAmazonSQS>();
        const string queueUrl = "https://sqs.us-east-2.amazonaws.com/0123456789ab/Example_Queue";
        var messageBody = JsonSerializer.Serialize(order);

        var request = new SendMessageRequest
        {
            MessageBody = messageBody,
            QueueUrl = queueUrl,
        };

        var response = await client.SendMessageAsync(request);

        IAmazonCloudWatchLogs _client = new AmazonCloudWatchLogsClient();
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            _ = await _client.PutLogEventsAsync(new PutLogEventsRequest()
            {
                LogGroupName = "",
                LogStreamName = "_logStream",
                LogEvents = new List<InputLogEvent>()
                {
                    new()
                    {
                        Message = JsonSerializer.Serialize(new
                        {
                            status = "OK",
                            order,
                            errorMessage = "",
                        }),
                        Timestamp = DateTime.Now
                    }
                }
            });
        }
        else
        {
            _ = await _client.PutLogEventsAsync(new PutLogEventsRequest()
            {
                LogGroupName = "",
                LogStreamName = "_logStream",
                LogEvents = new List<InputLogEvent>()
                {
                    new()
                    {
                        Message = JsonSerializer.Serialize(new
                        {
                            status = "Error",
                            order,
                            errorMessage = response.MD5OfMessageBody,
                        }),
                        Timestamp = DateTime.Now
                    }
                }
            });
        }

        return order;
    }
}