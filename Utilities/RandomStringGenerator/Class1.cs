using System.ComponentModel;
using System.Text;

namespace AK.Toolkit.Utilities ;
public static class RandomStringGenerator
{
    public enum OutputType
    {
        /// <summary>
        /// 0123456789
        /// </summary>
        Numbers,
        Alphabets,
        LowerCaseAlphabets,
        UppercaseAlphabets,
        AlphaNumbers,
        LowerCaseAlphaNumbers,
        UppercaseAlphaNumbers,
    }

    public static string GeneraterString(OutputType outputType, int length)
    {
        var source = GetSource(outputType);

        Random random = new();
        StringBuilder stringBuilder = new();

        for (var i = 0; i < length; i++)
        {
            _ = stringBuilder.Append(source[random.Next(source.Length)]);
        }

        return stringBuilder.ToString();
    }
    public static string GeneraterString(OutputType outputType, int minLength, int maxLength)
    {
        Random random = new();
        var length = random.Next(minLength, maxLength + 1);

        return GeneraterString(outputType, length);
    }

    private static string GetSource(OutputType outputType)
        => outputType switch
        {
            OutputType.Numbers => "0123456789",
            OutputType.Alphabets => "ABCDEFGHIZKLMNOPQRSTUVWXYZabcdefghigklmnopqstuvwxyz",
            OutputType.LowerCaseAlphabets => "abcdefghigklmnopqstuvwxyz",
            OutputType.UppercaseAlphabets => "ABCDEFGHIZKLMNOPQRSTUVWXYZ",
            OutputType.AlphaNumbers => "ABCDEFGHIZKLMNOPQRSTUVWXYZabcdefghigklmnopqstuvwxyz0123456789",
            OutputType.LowerCaseAlphaNumbers => "abcdefghigklmnopqstuvwxyz0123456789",
            OutputType.UppercaseAlphaNumbers => "ABCDEFGHIZKLMNOPQRSTUVWXYZ0123456789",
            _ => throw new InvalidEnumArgumentException("outputType", (int)outputType, typeof(OutputType))
        };
}
