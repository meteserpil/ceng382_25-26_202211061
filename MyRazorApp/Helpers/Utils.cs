// Helpers/Utils.cs
using System.Text.Json;

namespace MyRazorApp.Helpers
{
    public sealed class Utils
    {
        private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());
        
        public static Utils Instance => _instance.Value;
        
        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data, List<string>? selectedProperties = null)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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
    }
}