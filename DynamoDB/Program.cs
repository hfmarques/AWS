using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

var credentials = new BasicAWSCredentials("", "");
// Create client
var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

const string tableName = "Animals";
var animalsFileName = @"C:\Users\marqueh\Desenvolvimentos\Pessoal\AWS\DynamoDB\Animals.json";

// Add a single new Animal to the table.
// var animal = new Animal
// {
//     Id = 0,
//     Name = "Cat",
// };
//
// var item = new Dictionary<string, AttributeValue>
// {
//     ["Id"] = new() { N = animal.Id.ToString() },
//     ["Name"] = new() { S = animal.Name },
//     ["ScientificName"] = new() { S = animal.ScientificName ?? "" },
// };
//
// var request = new PutItemRequest
// {
//     TableName = tableName,
//     Item = item,
// };
//
// var response = await client.PutItemAsync(request);
// var success = response.HttpStatusCode == System.Net.HttpStatusCode.OK;
//
// Console.WriteLine(success ? $"Added {animal.Name} to the table." : "Could not add movie to table.");

if (!File.Exists(animalsFileName))
{
    throw new FileNotFoundException();
}

using var sr = new StreamReader(animalsFileName);
var json = sr.ReadToEnd();
var animals = JsonSerializer.Deserialize<List<Animal>>(json);

var context = new DynamoDBContext(client);

var bookBatch = context.CreateBatchWrite<Animal>();
bookBatch.AddPutItems(animals);

Console.WriteLine("Adding imported movies to the table.");
await bookBatch.ExecuteAsync();


public class Animal
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ScientificName { get; set; }
}


// var animalsTable = Table.LoadTable(client, "Animals");
// // Display Prompt
// Console.WriteLine("Table Scan " + Environment.NewLine);
// // Scan
// var scanFilter = new ScanFilter();
// var search = animalsTable.Scan(scanFilter);
// //All Fruit
// var fruitList = new List<string> ();
// //print
// do
// {
//     var documentList = await search.GetNextSetAsync();
//     foreach (var document in documentList)
//     {
//         var fruit = document.First().Value;
//         Console.WriteLine($"Fruit: {fruit}");
//         fruitList.Add(fruit); //Add scanned fruit to list
//     }
// } while (!search.IsDone);
// //Now pick a random fruit
// var random = new Random();
// var index = random.Next(fruitList.Count);
// Console.WriteLine($"Random Fruit: {fruitList[index]}");