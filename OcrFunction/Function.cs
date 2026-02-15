using Yandex.Cloud.Functions;

namespace OcrFunction;

public class Function : YcFunction<string, string>
{
    public string FunctionHandler(string request, Context context)
    {
        Console.WriteLine($"request: {request}");
        Console.WriteLine($"Context: {context}");
        return "Success";
    }
}