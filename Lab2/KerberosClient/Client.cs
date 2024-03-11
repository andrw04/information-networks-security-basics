using System.Text;
using System.Text.Json;
using Domain.Models;
using Serilog;

namespace KerberosClient;

public class Client
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<string> Register(string login)
    {
        var uri = new Uri("https://localhost:7159/api/AuthServer/register");
        var content = new StringContent($"\"{login}\"", Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var key = await response.Content.ReadAsStringAsync();
                return key;
            }
            else
            {
                Log.Error($"Error registering user: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error registering user: {ex.Message}");
        }

        return null;
    }

    public async Task<byte[]?> Authenticate(string login)
    {
        var uri = new Uri("https://localhost:7159/api/AuthServer/authenticate");
        var content = new StringContent($"\"{login}\"", Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Convert.FromBase64String(result);
            }
            else
            {
                Log.Error($"Error registering user: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error registering user: {ex.Message}");
        }

        return null;
    }

    public async Task<byte[]?> RequestTgs(TgsRequest request)
    {
        var uri = new Uri("https://localhost:7159/api/TicketGrantingServer/tgs");
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Convert.FromBase64String(result);
            }
            else
            {
                Log.Error($"Error getting ticket: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error requesting ticket: {ex.Message}");
        }

        return null;
    }

    public async Task<byte[]?> RequestService(ServiceRequest request)
    {
        var uri = new Uri("https://localhost:7159/api/ServiceServer/ss");
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Convert.FromBase64String(result);
            }
            else
            {
                Log.Error("");
            }
        }
        catch (Exception ex)
        {
            Log.Error($" : {ex.Message}");
        }

        return null;
    }
}
