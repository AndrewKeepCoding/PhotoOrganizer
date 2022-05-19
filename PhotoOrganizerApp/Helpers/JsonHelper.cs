using System.Text.Json;
using System.Threading.Tasks;

namespace PhotoOrganizings.Helpers;

public static class JsonHelper
{
    public static async Task<T?> ToObjectAsync<T>(string value)
    {
        return await Task.Run(() => JsonSerializer.Deserialize<T>(value));
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run(() => JsonSerializer.Serialize(value));
    }
}