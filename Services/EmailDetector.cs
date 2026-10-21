
using System.Text.RegularExpressions;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class EmailDetector : IDataDetector
{
    private static readonly Regex EmailRegex = new(
        @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public IEnumerable<DetectedData> Detect(string text)
    {
        foreach (Match match in EmailRegex.Matches(text))
        {
            yield return new DetectedData
            {
                Start = match.Index,
                Length = match.Length,
                Type = "EMAIL",
                Value = match.Value
            };
        }
    }
}
