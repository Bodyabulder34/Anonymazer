
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class FullNameDetector : IDataDetector
{
    private static readonly Regex Pattern = new(
        @"(?<![\p{L}])([А-ЯЁ][а-яё]{1,})\s+([А-ЯЁ][а-яё]{1,})\s+([А-ЯЁ][а-яё]{1,})(?![\p{L}])",
        RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in Pattern.Matches(text))
        {
            var surname = match.Groups[1].Value;
            var firstName = match.Groups[2].Value;
            var middleName = match.Groups[3].Value;

            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "FULL_NAME",
                Value = match.Value,
                Replacement = $"{surname} {firstName[0]}.{middleName[0]}.",
                Priority = 3
            };
        }
    }
}
