using System.Linq;
using StardewModdingAPI;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MonsterFindPlayerPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterFindPlayerPatch()
    {
        Original = RequireMethod<Monster>("findPlayer");
        Prefix.before = new[] {"Esca.FarmTypeManager"};
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("Esca.FarmTypeManager")]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer __result)
    {
        try
        {
            var targetId = __instance.ReadDataAs("Target", Game1.player.UniqueMultiplayerID);
            __result = ModEntry.HostState.FakeFarmers.TryGetValue(targetId, out var fakeFarmer)
                ? fakeFarmer
                : Game1.getFarmer(targetId);
        
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