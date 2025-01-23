namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Reflection;
using DaLion.Arsenal.Framework.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerFoundArtifactPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerFoundArtifactPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerFoundArtifactPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.foundArtifact));
    }

    #region harmony patches

    /// <summary>Trigger blueprint reward.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerFoundArtifactPrefix(Farmer __instance, int index)
    {
        if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue || index != JsonAssetsIntegration.DwarvishBlueprintIndex.Value)
        {
            return true; // run original logic
        }

        try
        {
            var found = __instance.Read(DataKeys.BlueprintsFound).ParseList<int>();
            int blueprint;
            if (!found.ContainsAny(WeaponIds.ElfBlade, WeaponIds.ForestSword))
            {
                blueprint = Game1.random.NextDouble() < 0.5 ? WeaponIds.ElfBlade : WeaponIds.ForestSword;
            }
            else
            {
                blueprint = found.Contains(WeaponIds.ElfBlade)
                    ? WeaponIds.ForestSword
                    : WeaponIds.ElfBlade;
            }

            __instance.Append(DataKeys.BlueprintsFound, blueprint.ToString());
            var count = __instance.Read(DataKeys.BlueprintsFound).ParseList<int>().Count;
            if (count == 1)
            {
                ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            }

            __instance.holdUpItemThenMessage(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
            if (Context.IsMultiplayer && Game1.player.mailReceived.Contains("clintForge"))
            {
                Broadcaster.SendPublicChat(I18n.Blueprint_Found_Global(__instance.Name));
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
