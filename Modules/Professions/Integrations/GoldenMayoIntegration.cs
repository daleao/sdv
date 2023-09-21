namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[ModRequirement("ughitsmegan.goldenmayoforJsonAssets", "[JA] Golden Mayo")]
internal sealed class GoldenMayoIntegration : ModIntegration<GoldenMayoIntegration, IJsonAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="GoldenMayoIntegration"/> class.</summary>
    internal GoldenMayoIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        this.ModApi.IdsAssigned += this.OnIdsAssigned;
        Log.D("[PROFS]: Registered the Golden Mayo integration.");
        return true;
    }

    /// <summary>Gets Ostrich Mayo ID.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        var index = this.ModApi.GetObjectId("Shiny Mayonnaise");
        if (index == -1)
        {
            Log.W("[PROFS]: Failed to get ID for Golden Mayo from Json Assets.");
            return;
        }

        Log.D($"[PROFS]: Json Assets ID {index} has been assigned to Golden Mayo.");
        Sets.AnimalDerivedProductIds = Sets.AnimalDerivedProductIds.Add(index);
    }
}
