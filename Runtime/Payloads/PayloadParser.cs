using System;
using System.Globalization;
using UnityEngine;

namespace UnityJS.Payloads
{
    public class PayloadParser
    {
        public static bool TryParse<TPayload>(string rawPayload, out TPayload parsedPayload)
        {
            if(TryParse(rawPayload, typeof(TPayload), out var payload))
            {
                parsedPayload = (TPayload)payload;
                return true;
            }
            parsedPayload = default;
            return false;
        }
        
        public static bool TryParse(string rawPayload, Type type, out object parsedPayload)
        {
            try
            {
                object payload;

                if (type == typeof(string))
                {
                    payload = rawPayload;
                }
                else if (type.IsEnum)
                {
                    payload = Enum.Parse(type, rawPayload, true);
                }
                else if (type == typeof(int))
                {
                    if (!int.TryParse(rawPayload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                        throw new FormatException($"Cannot parse '{rawPayload}' as int.");
                    payload = v;
                }
                else if (type == typeof(float))
                {
                    if (!float.TryParse(rawPayload, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v))
                        throw new FormatException($"Cannot parse '{rawPayload}' as float.");
                    payload = v;
                }
                else if (type == typeof(double))
                {
                    if (!double.TryParse(rawPayload, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v))
                        throw new FormatException($"Cannot parse '{rawPayload}' as double.");
                    payload = v;
                }
                else if (type == typeof(long))
                {
                    if (!long.TryParse(rawPayload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                        throw new FormatException($"Cannot parse '{rawPayload}' as long.");
                    payload = v;
                }
                else if (type == typeof(bool))
                {
                    if (!bool.TryParse(rawPayload, out var v))
                        throw new FormatException($"Cannot parse '{rawPayload}' as bool.");
                    payload = v;
                }
                else
                {
                    // Assume JSON and attempt deserialization
                    payload = JsonUtility.FromJson(rawPayload, type);
                }

                parsedPayload = payload;
                return true;
            }
            catch (Exception ex)
            {
                JSInstance.LogError($"PayloadParser<{type.Name}> failed to convert payload '{rawPayload}': {ex}");
                parsedPayload = default;
                return false;
            }
        }
        
        public static bool TryStringify<T>(T value, out string result)
        {
            try
            {
                if (value == null)
                {
                    result = "null";
                    return true;
                }

                Type type = typeof(T);

                if (type == typeof(string))
                {
                    result = (string)(object)value;
                }
                else if (type.IsEnum)
                {
                    result = value.ToString();
                }
                else if (type == typeof(int) || type == typeof(float) || type == typeof(double) ||
                         type == typeof(long) || type == typeof(bool))
                {
                    result = Convert.ToString(value, CultureInfo.InvariantCulture);
                }
                else
                {
                    // Assume it's a complex object and try JSON serialization
                    result = JsonUtility.ToJson(value);
                }

                return true;
            }
            catch (Exception ex)
            {
                JSInstance.LogError($"ResultStringifier<{typeof(T).Name}> failed to stringify value: {ex}");
                result = null;
                return false;
            }
        }
    }
}