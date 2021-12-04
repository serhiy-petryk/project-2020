using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using DGCore.Helpers;

namespace DGCore.Utils
{
    public static class Json
    {

        public static JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(), new JsonTimeSpanConverter() }
        };

        public static T CloneJson<T>(this T source)
        {
            if (ReferenceEquals(source, null))
                return default(T);

            var json = JsonSerializer.Serialize(source, DefaultJsonOptions);
            var clone = JsonSerializer.Deserialize<T>(json, DefaultJsonOptions);
            ConvertJsonElements(clone);
            return clone;
        }

        /*/// From https://stackoverflow.com/questions/78536/deep-cloning-objects
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
                return default(T);

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }*/

        // Properties of object type with any value are deserialize into System.Text.Json.JsonElement
        // The below method tries to convert JsonElements into values for some types
        public static void ConvertJsonElements(object obj)
        {
            var objType = obj?.GetType();
            if (objType == null || objType.IsPrimitive || objType == typeof(string))
                return;

            // var properties = objType.GetProperties();
            var properties = PD.MemberDescriptorUtils.GetPublicProperties(objType);
            foreach (var property in properties)
            {
                var propValue = property.GetValue(obj, null);
                if (propValue == null || property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                    continue;
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var enumerable = (IEnumerable)propValue;
                    foreach (var child in enumerable)
                        ConvertJsonElements(child);
                }
                else if (propValue is JsonElement json)
                {
                    switch (json.ValueKind)
                    {
                        case JsonValueKind.String:
                            property.SetValue(obj, json.GetString());
                            break;
                        case JsonValueKind.Number:
                            property.SetValue(obj, json.GetDecimal());
                            break;
                        case JsonValueKind.False:
                        case JsonValueKind.True:
                            property.SetValue(obj, json.GetBoolean());
                            break;
                        default:
                            throw new Exception($"Trap!!!. ConvertJsonElements not defined for {json.ValueKind}");
                    }
                }
                else
                    ConvertJsonElements(propValue);
            }
        }
    }
}
