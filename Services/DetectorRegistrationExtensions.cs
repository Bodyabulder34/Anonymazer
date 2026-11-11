
using Microsoft.Extensions.DependencyInjection;

namespace Anonymazer.Services;

public static class DetectorRegistrationExtensions
{
    public static IServiceCollection AddDataDetectors(
        this IServiceCollection services)
    {
        var detectorTypes = typeof(IDataDetector).Assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                typeof(IDataDetector).IsAssignableFrom(type));

        foreach (var detectorType in detectorTypes)
        {
            services.AddScoped(typeof(IDataDetector), detectorType);
        }

        return services;
    }
}
