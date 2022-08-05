namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.JsonAssets;
using System;
using System.IO;

#endregion using directives

internal sealed class JsonAssetsIntegration : BaseIntegration<IJsonAssetsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public JsonAssetsIntegration(IModRegistry modRegistry)
        : base("JsonAssets", "spacechase0.JsonAssets", "1.10.7", modRegistry) { }

    /// <summary>Register the Garnet and Garnet Ring items.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.JsonAssetsApi = ModApi;
        ModApi.LoadAssets(Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "json-assets"), ModEntry.i18n);
        ModApi.IdsAssigned += OnIdsAssigned;
    }

    /// <summary>Get assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (ModApi is null) return;
        
        ModEntry.GarnetIndex = ModApi.GetObjectId("Garnet");
        ModEntry.GarnetRingIndex = ModApi.GetObjectId("Garnet Ring");
    }
}