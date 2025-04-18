using System.Text.Json;
using System.Text.Encodings.Web;

namespace MyRazorApp.Helpers
{
    public sealed class Utils
    {
        private static readonly Lazy<Utils> _instance = new(() => new Utils());

        public static Utils Instance => _instance.Value;

        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data, List<string>? selectedProperties = null)
        {
            try 
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                if (selectedProperties == null || !selectedProperties.Any())
                {
                    return JsonSerializer.Serialize(data, options);
                }

                var filteredData = data.Select(item =>
                {
                    var dictionary = new Dictionary<string, object?>();
                    var properties = typeof(T).GetProperties();

                    foreach (var prop in properties)
                    {
                        if (selectedProperties.Contains(prop.Name))
                        {
                            dictionary[prop.Name] = prop.GetValue(item);
                        }
                    }
                    return dictionary;
                });

                return JsonSerializer.Serialize(filteredData, options);
            }
            catch (Exception ex)
            {
                return "{\"error\": \"Failed to generate JSON: " + ex.Message + "\"}";
            }
        }
    }
}