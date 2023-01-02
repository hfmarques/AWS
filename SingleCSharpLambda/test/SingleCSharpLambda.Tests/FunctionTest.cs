using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace SingleCSharpLambda.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var result = function.FunctionHandler("hello world", context);

        Assert.Equal("hello world".ToUpper(), result);
        Assert.Equal("HELLO WORLD", result);
    }
}