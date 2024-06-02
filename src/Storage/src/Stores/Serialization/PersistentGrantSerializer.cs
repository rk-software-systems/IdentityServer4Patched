// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace IdentityServer4.Stores.Serialization;

/// <summary>
/// JSON-based persisted grant serializer
/// </summary>
/// <seealso cref="IPersistentGrantSerializer" />
public class PersistentGrantSerializer : IPersistentGrantSerializer
{
    private static readonly JsonSerializerOptions _settings;

    static PersistentGrantSerializer()
    {
        _settings = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                   (typeInfo) =>
                   {
                        if(typeInfo.Properties.Count > 0)
                        {

                            var resultProperties = typeInfo.Properties.Where(x=>x.Set != null).ToList();
                            typeInfo.Properties.Clear();
                            foreach(var property in resultProperties)
                            {
                                typeInfo.Properties.Add(property);
                            }
                        }
                   }
                },
            },
        };
        _settings.Converters.Add(new ClaimConverter());
        _settings.Converters.Add(new ClaimsPrincipalConverter());
    }

    /// <summary>
    /// Serializes the specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _settings);
    }

    /// <summary>
    /// Deserializes the specified string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _settings);
    }
}