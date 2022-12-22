using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

var credentials = new BasicAWSCredentials("", "");
var client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
const string bucketName = "hfmbucket";
const string objectName = "object123.txt";
const string filePath = "C:\\Users\\marqueh\\Desenvolvimentos\\Pessoal\\AWS\\S3\\";
    
var requestPut = new PutObjectRequest
{
    BucketName = bucketName,
    Key = objectName,
    FilePath = filePath + objectName,
};

var responsePut = await client.PutObjectAsync(requestPut);
Console.WriteLine(responsePut.HttpStatusCode == System.Net.HttpStatusCode.OK
    ? $"Successfully uploaded {objectName} to {bucketName}."
    : $"Could not upload {objectName} to {bucketName}.");
    
// Create a GetObject request
var requestGet = new GetObjectRequest
{
    BucketName = bucketName,
    Key = objectName,
};

// Issue request and remember to dispose of the response
using var responseGet = await client.GetObjectAsync(requestGet);

try
{
    // Save object to local file
    await responseGet.WriteResponseStreamToFileAsync($"{filePath}\\saved_{objectName}", true, CancellationToken.None);
}
catch (AmazonS3Exception ex)
{
    Console.WriteLine($"Error saving {objectName}: {ex.Message}");
}