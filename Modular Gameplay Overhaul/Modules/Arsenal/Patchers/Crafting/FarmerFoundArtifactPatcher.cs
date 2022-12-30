namespace DaLion.Overhaul.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
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
    internal FarmerFoundArtifactPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.foundArtifact));
    }

    #region harmony patches

    /// <summary>Trigger blueprint reward.</summary>
    [HarmonyPrefix]
    private static bool FarmerFoundArtifactPrefix(Farmer __instance, int index)
    {
        if (!Globals.DwarvishBlueprintIndex.HasValue || index != Globals.DwarvishBlueprintIndex.Value)
        {
            return true; // run original logic
        }

        try
        {
            var found = __instance.Read(DataFields.BlueprintsFound).ParseList<int>();
            int blueprint;
            if (!found.ContainsAny(Constants.ElfBladeIndex, Constants.ForestSwordIndex))
            {
                blueprint = Game1.random.NextDouble() < 0.5 ? Constants.ElfBladeIndex : Constants.ForestSwordIndex;
            }
            else
            {
                blueprint = found.Contains(Constants.ElfBladeIndex)
                    ? Constants.ForestSwordIndex
                    : Constants.ElfBladeIndex;
            }

            __instance.Append(DataFields.BlueprintsFound, blueprint.ToString());
            var count = __instance.Read(DataFields.BlueprintsFound).ParseList<int>().Count;
            switch (count)
            {
                case 8:
                    __instance.completeQuest(Constants.ForgeNextQuestId);
                    break;
                case 1:
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
                    break;
            }

            __instance.holdUpItemThenMessage(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
            if (Context.IsMultiplayer)
            {
                Broadcaster.SendPublicChat(I18n.Get("blueprint.found.global", new { who = __instance.Name }));
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
