
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class PassportDetector : IDataDetector
{
    private static readonly Regex PassportRegex = new(
        @"(?<!\d)(?:\d{2}\s\d{2}\s\d{6}|\d{4}\s?\d{6})(?!\d)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in PassportRegex.Matches(text))
        {
            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "PASSPORT",
                Value = match.Value
            };
        }
    }
}
