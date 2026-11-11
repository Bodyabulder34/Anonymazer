
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class PhoneDetector : IDataDetector
{
    private static readonly Regex PhoneRegex = new(
        @"(?<!\w)(?:(?:\+7|7|8)(?:[\s().-]*\d){10}|9(?:[\s().-]*\d){9})(?!\w)",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in PhoneRegex.Matches(text))
        {
            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "PHONE",
                Value = match.Value,
                Replacement = "[PHONE]",
                Priority = 100
            };
        }
    }
}
