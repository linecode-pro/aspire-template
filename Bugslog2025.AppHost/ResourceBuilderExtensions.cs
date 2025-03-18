using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugslog2025.AppHost
{
    internal static class ResourceBuilderExtensions
    {
        internal static IResourceBuilder<T> WithSwaggerUI<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
        {
            return builder.WithOpenApiDocs("swagger-ui-docs", "Swagger API Documentation", "swagger");
        }

        private static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder,
            string name,
            string displayName,
            string openApiUiPath)
            where T : IResourceWithEndpoints
        {
            return builder.WithCommand(
                name,
                displayName,
                executeCommand: async _ =>
                {
                    try
                    {
                        // Base URL
                        var endpoint = builder?.GetEndpoint("https") ?? throw new Exception("Не задан endpoint https");

                        var url = $"{endpoint.Url}/{openApiUiPath}";

                        await Task.Run(() => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }));

                        return new ExecuteCommandResult { Success = true };
                    }
                    catch (Exception ex)
                    {
                        return new ExecuteCommandResult { Success = false, ErrorMessage = ex.Message };
                    }
                },
                updateState: context => context.ResourceSnapshot.HealthStatus == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ?
                    ResourceCommandState.Enabled : ResourceCommandState.Disabled,
                iconName: "Document",
                iconVariant: IconVariant.Filled);
        }
    }
}
