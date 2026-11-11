
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class SnilsDetector : IDataDetector
{
    private static readonly Regex Pattern = new(
        @"(?<!\d)(\d{3})-?(\d{3})-?(\d{3})\s?(\d{2})(?!\d)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in Pattern.Matches(text))
        {
            var digits = Regex.Replace(match.Value, @"\D", "");

            if (!IsValidSnils(digits))
                continue;

            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "SNILS",
                Value = match.Value,
                Replacement = "[SNILS]",
                Priority = 1
            };
        }
    }

    private static bool IsValidSnils(string digits)
    {
        if (digits.Length != 11)
            return false;

        var number = digits[..9];
        var control = int.Parse(digits[9..]);

        var sum = 0;

        for (var i = 0; i < 9; i++)
        {
            sum += (digits[i] - '0') * (9 - i);
        }

        var expected = sum switch
        {
            < 100 => sum,
            100 or 101 => 0,
            _ => sum % 101 switch
            {
                100 => 0,
                var value => value
            }
        };

        return control == expected;
    }
}
