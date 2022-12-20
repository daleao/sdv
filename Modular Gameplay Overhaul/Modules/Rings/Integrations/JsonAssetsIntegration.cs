namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.IO;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[RequiresMod("spacechase0.JsonAssets", "Json Assets", "1.10.7")]
internal sealed class JsonAssetsIntegration : ModIntegration<JsonAssetsIntegration, IJsonAssetsApi>
{
    internal JsonAssetsIntegration()
        : base("spacechase0.JsonAssets", "Json Assets", "1.10.7", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (this.IsLoaded)
        {
            var subFolder = VanillaTweaksIntegration.Instance?.RingsCategoryEnabled == true
                ? "VanillaTweaks"
                : BetterRingsIntegration.Instance?.IsLoaded == true
                    ? "BetterRings" : "Vanilla";
            this.ModApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Rings", subFolder), I18n);
            this.ModApi.IdsAssigned += this.OnIdsAssigned;
            return true;
        }

        RingsModule.Config.TheOneInfinityBand = false;
        ModHelper.WriteConfig(ModEntry.Config);
        return false;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (this.ModApi is null)
        {
            return;
        }

        Globals.GarnetIndex = this.ModApi.GetObjectId("Garnet");
        Globals.GarnetRingIndex = this.ModApi.GetObjectId("Garnet Ring");
        Globals.InfinityBandIndex = this.ModApi.GetObjectId("Infinity Band");
        Log.D("The IDs for custom items in the Rings module have been assigned.");
    }
}
