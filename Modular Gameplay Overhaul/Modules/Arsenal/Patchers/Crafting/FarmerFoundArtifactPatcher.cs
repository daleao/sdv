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
            var found = __instance.Read(DataKeys.BlueprintsFound).ParseList<int>();
            int blueprint;
            if (!found.ContainsAny(ItemIDs.ElfBlade, ItemIDs.ForestSword))
            {
                blueprint = Game1.random.NextDouble() < 0.5 ? ItemIDs.ElfBlade : ItemIDs.ForestSword;
            }
            else
            {
                blueprint = found.Contains(ItemIDs.ElfBlade)
                    ? ItemIDs.ForestSword
                    : ItemIDs.ElfBlade;
            }

            __instance.Append(DataKeys.BlueprintsFound, blueprint.ToString());
            var count = __instance.Read(DataKeys.BlueprintsFound).ParseList<int>().Count;
            switch (count)
            {
                case 8:
                    __instance.completeQuest((int)Quest.ForgeNext);
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
