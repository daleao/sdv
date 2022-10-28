namespace DaLion.Redux;

#region using directives

using Core.Integrations;
using DaLion.Shared.Integrations.DynamicGameAssets;
using DaLion.Shared.Integrations.JsonAssets;
using DaLion.Shared.Integrations.LoveOfCooking;
using DaLion.Shared.Integrations.LuckSkill;
using DaLion.Shared.Integrations.SpaceCore;
using Newtonsoft.Json.Linq;
using Shared.Integrations.WearMoreRings;

#endregion using directives

/// <summary>Holds APIs and other variables related to third-party mod integrations.</summary>
internal static class Integrations
{
    /// <summary>Gets or sets the <see cref="ICookingSkillApi"/>.</summary>
    internal static ICookingSkillApi? CookingSkillApi { get; set; }

    /// <summary>Gets or sets the <see cref="IDynamicGameAssetsApi"/>.</summary>
    internal static IDynamicGameAssetsApi? DynamicGameAssetsApi { get; set; }

    /// <summary>Gets or sets the <see cref="IJsonAssetsApi"/>.</summary>
    internal static IJsonAssetsApi? JsonAssetsApi { get; set; }

    /// <summary>Gets or sets the <see cref="ILuckSkillApi"/>.</summary>
    internal static ILuckSkillApi? LuckSkillApi { get; set; }

    /// <summary>Gets or sets the <see cref="ISpaceCoreApi"/>.</summary>
    internal static ISpaceCoreApi? SpaceCoreApi { get; set; }

    /// <summary>Gets or sets the <see cref="IWearMoreRingsApi"/>.</summary>
    internal static IWearMoreRingsApi? WearMoreRingsApi { get; set; }

    /// <summary>Gets or sets the config object for Stardew Valley Expanded.</summary>
    internal static JObject? SveConfig { get; set; }

    /// <summary>Gets or sets a value indicating whether Better Rings mod is loaded in the current game session.</summary>
    internal static bool IsBetterRingsLoaded { get; set; }

    /// <summary>Gets or sets a value indicating whether Moon Misadventures mod is loaded in the current game session.</summary>
    internal static bool IsMoonMisadventuresLoaded { get; set; }

    /// <summary>Gets or sets the <see cref="GenericModConfigMenuIntegration"/> instance.</summary>
    internal static GenericModConfigMenuIntegration GmcmIntegration { get; set; } = null!; // set on game launched
}
