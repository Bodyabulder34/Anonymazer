
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class BankCardDetector : IDataDetector
{
    private static readonly Regex Pattern = new(
        @"(?<!\d)(?:\d[ -]?){12,18}\d(?!\d)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in Pattern.Matches(text))
        {
            var digits = Regex.Replace(match.Value, @"[ -]", "");

            if (!IsValidCardNumber(digits))
                continue;

            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "BANK_CARD",
                Value = match.Value,
                Replacement = digits[..4] + new string('*', digits.Length - 4),
                Priority = 1
            };
        }
    }

    private static bool IsValidCardNumber(string number)
    {
        if (number.Length is < 13 or > 19)
            return false;

        var sum = 0;
        var doubleDigit = false;

        for (var i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';

            if (doubleDigit)
            {
                digit *= 2;

                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            doubleDigit = !doubleDigit;
        }

        return sum % 10 == 0;
    }
}
