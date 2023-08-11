using System.Diagnostics;
using System.Runtime.CompilerServices;
using Iggy.Net.Enums;

namespace Iggy.Net.Extensions;

internal static class StringExtensions
{
    internal static string ToSnakeCase(this string input)
    {
        Debug.Assert(!string.IsNullOrEmpty(input));
        if (CountUppercaseLetters(input) == 0)
            return input.ToLower();
       
        var len = input.Length + CountUppercaseLetters(input) - 1;
        return string.Create(len, input, (span, value) =>
        {
            value.AsSpan().CopyTo(span);
            span[0] = char.ToLower(span[0]);
	      
            for (int i = 0; i < len; ++i)
            {
                if (char.IsUpper(span[i]))
                {
                    span[i] = char.ToLower(span[i]);
                    span[i..].ShiftSliceRight();
                    span[i] = '_';
                }
            }
        });
    }
    
    private static int CountUppercaseLetters(string input)
    {
        return input.Count(char.IsUpper);
    }
    
    private static void ShiftSliceRight(this Span<char> slice)
    {
        for (int i = slice.Length - 2; i >= 0; i--)
        {
            slice[i + 1] = slice[i];
        }
    }
}

[InterpolatedStringHandler]
internal ref struct MessageRequestInterpolationHandler
{
    private DefaultInterpolatedStringHandler _innerHandler;
    internal MessageRequestInterpolationHandler(int literalLength, int formattedCount)
    {
        _innerHandler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    }

    internal void AppendLiteral(string message)
    {
        _innerHandler.AppendLiteral(message);	
    }
    internal void AppendFormatted<T>(T t)
    {
        switch (t)
        {
            case MessagePolling pollingStrat:
            {
                var str = pollingStrat switch
                {
                    MessagePolling.Offset => "offset",
                    MessagePolling.Timestamp => "timestamp",
                    MessagePolling.First => "first",
                    MessagePolling.Last => "last",
                    MessagePolling.Next => "next",
                    _ => throw new ArgumentOutOfRangeException()
                };
                _innerHandler.AppendFormatted(str);
                break;
            }
            case bool tBool:
                _innerHandler.AppendFormatted(tBool.ToString().ToLower());
                break;
            default:
                _innerHandler.AppendFormatted(t);
                break;
        }
    }

    public override string ToString()
    {
        return _innerHandler.ToString();
    }

    public string ToStringAndClear()
    {
        return _innerHandler.ToStringAndClear();	
    }
}