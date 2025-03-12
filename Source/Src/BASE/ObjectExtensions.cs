﻿using System.Reflection;
using System.Text.Json;

namespace BASE;

public static class ObjectExtensions
    {
    public static bool UpdateProperty<T>(ref T existingValue, T newValue)
        {
        if (!Equals(existingValue, newValue))
            {
            existingValue = newValue;
            return true;
            }
        return false;
        }

    public static T RemoveProperty<T>(this T source, string propertyName)
        {
        // Create a new object of the same type (T)
        T result = Activator.CreateInstance<T>();

        // Copy all properties from the source object to the result object except the specified property
        foreach (var propertyInfo in typeof(T).GetProperties())
            {
            if (propertyInfo.Name != propertyName)
                {
                propertyInfo.SetValue(result, propertyInfo.GetValue(source));
                }
            }

        return result;
        }

    public static T Clone<T>(T source)
        {
        if (source == null)
            {
            throw new ArgumentNullException(nameof(source));
            }

        string json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
        }

    public static T CloneExcludingNested<T>(T source)
        {
        if (source == null)
            {
            throw new ArgumentNullException(nameof(source));
            }

        var clonedObject = Activator.CreateInstance<T>();

        foreach (var property in typeof(T).GetProperties())
            {
            // Check if the property is not a nested object (not a class)
            if (!property.PropertyType.IsClass || property.PropertyType == typeof(string))
                {
                property.SetValue(clonedObject, property.GetValue(source));
                }
            }

        return clonedObject;
        }

    public static void ResetToDefault<T>(T obj)
        {
        if (obj == null) return;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
            {
            if (property.CanWrite)
                {
                var defaultValue = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
                property.SetValue(obj, defaultValue);
                }
            }
        }
    }