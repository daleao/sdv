namespace DaLion.Shared.Configs;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Base class for a mod's config settings.</summary>
public abstract class Config
{
    private static readonly Lazy<JsonSerializerSettings> JsonSerializerSettings =
        new(() => ModHelper.Data.GetJsonSerializerSettings());

    /// <summary>Validate the config settings, replacing invalid values if necessary.</summary>
    /// <returns><see langword="true"/> if all config settings are valid and don't need rewriting, otherwise <see langword="false"/>.</returns>
    public virtual bool Validate()
    {
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, JsonSerializerSettings.Value);
    }
}
