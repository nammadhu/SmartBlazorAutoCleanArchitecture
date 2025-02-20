using System.Text.Json;
using System.Text.Json.Serialization;

namespace PublicCommon;

public static class JsonExtensions
{
    public static T? CloneBySerializing<T>(this T obj)
    {
        if (obj == null) return default;

        try
        {
            string serialized = JsonSerializer.Serialize(obj, CONSTANTS.DefaultSerializationJsonOptions);
            return JsonSerializer.Deserialize<T>(serialized, CONSTANTS.DefaultSerializationJsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Clone failed for type {typeof(T)} with exception {ex}");
            return default;
        }
    }

    public static string? Serialize<T>(this T obj)
    {
        return obj != null ? JsonSerializer.Serialize(obj, CONSTANTS.DefaultSerializationJsonOptions) : null;
    }

    public static T? DeSerialize<T>(this string? jsonString)
    {
        return TryDeserialize(jsonString, out T? result) ? result : default(T);
    }

    public static bool TryDeserialize<T>(string? json, out T? result, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            result = default;
            return false;
        }

        try
        {
            result = JsonSerializer.Deserialize<T>(json, options ?? CONSTANTS.DefaultSerializationJsonOptions);
            return true;
        }
        catch (JsonException e)
        {
            System.Diagnostics.Trace.TraceError($"JSON error: {e}");
            Console.WriteLine($"JSON error: {e}");
        }
        catch (Exception e)
        {
            System.Diagnostics.Trace.TraceError($"General error: {e}");
            Console.WriteLine($"General error: {e}");
        }

        result = default;
        return false;
    }

    public static string CleanObject(object obj)
    {//this can be used for offline storage
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        return JsonSerializer.Serialize(obj, options);
    }

    //public static bool TryDeserialize<T>(string json, out T result)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(json))
    //        {
    //            result = default;
    //            return false;
    //        }
    //        result = JsonSerializer.Deserialize<T>(json);
    //        return true;
    //    }
    //    //catch (JsonException)
    //    //{
    //    //    result = default;
    //    //    return false;
    //    //}
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e.ToString());
    //        result = default;
    //        return false;
    //    }
    //}
}