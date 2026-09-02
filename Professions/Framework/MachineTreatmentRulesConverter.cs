namespace DaLion.Professions.Framework;

#region using directives

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion using directives

/// <inheritdoc />
internal sealed class MachineTreatmentRulesConverter : JsonConverter<MachineTreatmentRules>
{
    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override MachineTreatmentRules ReadJson(
        JsonReader reader,
        Type objectType,
        MachineTreatmentRules? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var @default =
            obj.TryGetValue("default", StringComparison.OrdinalIgnoreCase, out JToken? defaultToken)
                ? ParseCategory(defaultToken)
                : MachineTreatmentRegistry.None;

        Dictionary<string, MachineTreatment> overrides = [];
        foreach ((var key, var value) in obj)
        {
            if (key.Equals("default", StringComparison.OrdinalIgnoreCase) || value is null)
            {
                continue;
            }

            overrides[key] = ParseCategory(value);
        }

        return new MachineTreatmentRules(@default, overrides);
    }

    /// <inheritdoc />
    public override void WriteJson(
        JsonWriter writer,
        MachineTreatmentRules? value,
        JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }

    private static MachineTreatment ParseCategory(JToken token)
    {
        if (token.Type == JTokenType.Null)
        {
            return MachineTreatmentRegistry.None;
        }

        string? value = token.Value<string>();
        return value is null
            ? throw new JsonSerializationException(
                $"Invalid machine treatment category '{value}'.")
            : MachineTreatmentRegistry.GetOrRegister(value);
    }
}
