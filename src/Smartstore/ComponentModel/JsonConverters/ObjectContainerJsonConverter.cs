﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Smartstore.ComponentModel.JsonConverters
{
    public class ObjectContainerJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(IObjectContainer).IsAssignableFrom(objectType);

        public override bool CanRead
            => true;

        public override bool CanWrite
            => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = (IObjectContainer)(existingValue ?? Activator.CreateInstance(objectType));

            serializer.Populate(reader, result);
            SanitizeValue(result, serializer);

            return result;
        }

        /// <summary>
        /// Checks whether <see cref="IObjectContainer.Value"/> is of type <see cref="JToken"/> (Array or Object)
        /// and converts instance to <see cref="IObjectContainer.ValueType"/>.
        /// </summary>
        protected virtual IObjectContainer SanitizeValue(IObjectContainer result, JsonSerializer serializer)
        {
            if (result.Value is IObjectContainer container)
            {
                // Recursion
                SanitizeValue(container, serializer);
                return result;
            }

            if (result.Value is not JToken valueToken)
            {
                return result;
            }

            var valueJson = valueToken.ToString(Formatting.None);

            JsonConverter converter = null;

            if (result.ValueType.TryGetAttribute<JsonConverterAttribute>(true, out var converterAttribute))
            {
                converter = (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType);
            }

            if (converter == null)
            {
                converter = serializer.ContractResolver.ResolveContract(result.ValueType).Converter;
            }

            result.Value = converter != null
                ? JsonConvert.DeserializeObject(valueJson, result.ValueType, converter)
                : JsonConvert.DeserializeObject(valueJson, result.ValueType);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
