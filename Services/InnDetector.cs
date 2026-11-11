
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class InnDetector : IDataDetector
{
    private static readonly Regex Pattern = new(
        @"(?<!\d)\d{12}(?!\d)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in Pattern.Matches(text))
        {
            if (!IsValidInn(match.Value))
                continue;

            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "INN",
                Value = match.Value,
                Replacement = "[INN]",
                Priority = 2
            };
        }
    }

    private static bool IsValidInn(string inn)
    {
        if (inn.Length != 12)
            return false;

        int[] firstWeights = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        int[] secondWeights = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };

        var firstCheck = CalculateCheckDigit(inn, firstWeights);
        var secondCheck = CalculateCheckDigit(inn, secondWeights);

        return firstCheck == inn[10] - '0' &&
               secondCheck == inn[11] - '0';
    }

    private static int CalculateCheckDigit(string inn, int[] weights)
    {
        var sum = 0;

        for (var i = 0; i < weights.Length; i++)
        {
            sum += (inn[i] - '0') * weights[i];
        }

        return sum % 11 % 10;
    }
}
