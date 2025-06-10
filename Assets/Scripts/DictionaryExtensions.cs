using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public static class DictionaryExtensions
{
    [CanBeNull]
    public static TV Get<TK, TV>(this IDictionary<TK, TV> dict, [CanBeNull] TK key, TV defaultValue = default)
    {
        if (key == null)
        {
            return defaultValue;
        }

        return (!dict.TryGetValue(key, out TV tv)) ? defaultValue : tv;
    }

    public static bool TryGetValueWithSubStringKey<T>(this Dictionary<string, T> source, [NotNull] string key, out T value)
    {
        if (source.TryGetValue(key, out value))
        {
            return true;
        }

        foreach (KeyValuePair<string, T> keyValuePair in source)
        {
            bool ignoreCase = key.IndexOf(keyValuePair.Key, StringComparison.OrdinalIgnoreCase) >= 0;
            if (keyValuePair.Key.Length > 0 && ignoreCase)
            {
                value = keyValuePair.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source is null");
        }

        if (target == null)
        {
            return;
        }

        foreach (KeyValuePair<TKey, TValue> keyValuePair in target)
        {
            source[keyValuePair.Key] = keyValuePair.Value;
        }
    }
}
