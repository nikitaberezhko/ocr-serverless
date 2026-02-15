namespace Tests;

public class FirstTest
{
    [Fact]
    public void FunctionHandler_ShouldReturnSuccess()
    {
        var sut = new OcrFunction.Function();
        var result = sut.FunctionHandler("test-request", null!);

        Assert.Equal("Success", result);
    }
}