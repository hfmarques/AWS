using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

var credentials = new BasicAWSCredentials("", "");
var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
var context = new DynamoDBContext(client);

// Add a single new Animal to the table.
var animal = new Animal
{
    Id = Guid.NewGuid(),
    Name = "Cat, tiger",
    ScientificName = "Dasyurus maculatus"
};

await context.SaveAsync(animal);

Console.WriteLine($"Added {animal.Name} to the table.");

// Retrieve the animal from the ProductCatalog table.
var animalRetrieved = await context.LoadAsync<Animal>(animal.Id);

// Update some properties.
animalRetrieved.Name = "Cat";

await context.SaveAsync(animalRetrieved);

//Scan the table
var scs = new List<ScanCondition>();
var sc1 = new ScanCondition("Name", ScanOperator.Equal, "Cat");
scs.Add(sc1);

var animalsRetrieved = await context.ScanAsync<Animal>(scs).GetRemainingAsync();

Console.WriteLine($"Animal retrieved name {animalsRetrieved.First().Name}");

// Delete the animal.
await context.DeleteAsync<Animal>(animal.Id);

// Try to retrieve deleted animal. It should return null.
var deletedAnimal = await context.LoadAsync<Animal>(animal.Id, new DynamoDBContextConfig
{
    ConsistentRead = true,
});

if (deletedAnimal == null)
{
    Console.WriteLine("animal is deleted");
}

[DynamoDBTable("Animals")]
public class Animal
{
    [DynamoDBHashKey]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ScientificName { get; set; }
}