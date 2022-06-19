#nullable enable
using System.Collections;
using HarmonyLib;
using StardewModdingAPI;

namespace FlowerMeads;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>Construct an instance.</summary>
    public ModEntry()
    {
        // add mead entry to BAGI's ContentSourceManager dictionary
        // this will fix a likely KeyNotFoundException
        try
        {
            var artisanGoodToSourceTypeDict = (IDictionary) AccessTools
                .TypeByName("BetterArtisanGoodIcons.Content.ContentSourceManager")
                .RequireField("artisanGoodToSourceType").GetValue(null)!;
            artisanGoodToSourceTypeDict.Add(Globals.MeadAsArtisanGoodEnum, "Flowers");
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // apply patches
        var harmony = new Harmony(ModManifest.UniqueID);
        HarmonyPatcher.Apply(harmony);

        if (helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            HarmonyPatcher.ApplyAutomate(harmony);

        if (helper.ModRegistry.IsLoaded("cat.betterartisangoodicons"))
            HarmonyPatcher.ApplyBAGI(harmony);
    }
}