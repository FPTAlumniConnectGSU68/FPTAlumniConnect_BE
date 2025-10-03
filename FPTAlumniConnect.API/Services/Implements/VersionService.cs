using FPTAlumniConnect.API;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace StringUpdateApi.Services;

public class VersionService
{
    private readonly IConfiguration _configuration;
    private readonly string _configFilePath;

    public VersionService(IConfiguration configuration)
    {
        _configuration = configuration;
        _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
    }

    public string GetVersion()
    {
        // Get version from appsettings.json
        return _configuration["AppVersion"] ?? "1.0.0";
    }

    public string UpdateVersionToCurrentDate()
    {
        try
        {
            // Generate new version based on current date (YYYY.MM.DD)
            var newVersion = TimeHelper.NowInVietnam().ToString("yyyy.MM.dd.HH.mm");

            // Load appsettings.json
            var json = File.ReadAllText(_configFilePath);
            var jsonNode = JsonNode.Parse(json)
                ?? throw new InvalidOperationException("Failed to parse appsettings.json");

            // Update AppVersion
            jsonNode["AppVersion"] = newVersion;

            // Write back to appsettings.json
            var updatedJson = jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, updatedJson);

            // Reload configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var updatedConfig = configBuilder.Build();
            (_configuration as IConfigurationRoot)?.Reload();

            return newVersion;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"JSON parsing error in appsettings.json: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update version: {ex.Message}", ex);
        }
    }
}