using Newtonsoft.Json;

namespace ApiCommons.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to its equivalent JSON string representation.
        /// </summary>
        /// <param name="obj">The object to convert to JSON.</param>
        /// <param name="jsonSerializerSettings">Optional JSON serializer settings to customize the serialization behavior.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object obj, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }
    }
}
