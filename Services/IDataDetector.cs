
using Anonymazer.Models;

namespace Anonymazer.Services;

public interface IDataDetector
{
    IEnumerable<DetectedData> Detect(string text);
}
