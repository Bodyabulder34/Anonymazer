
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class PhoneDetector : IDataDetector
{
    private static readonly Regex PhoneRegex = new(
        @"(?<!\w)\+?\d(?:[\d\s().-]*\d)?(?!\w)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in PhoneRegex.Matches(text))
        {
            string digits = Regex.Replace(match.Value, @"\D", "");

            if (digits.Length < 10 || digits.Length > 15)
                continue;

            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "PHONE",
                Value = match.Value
            };
        }
    }
}
