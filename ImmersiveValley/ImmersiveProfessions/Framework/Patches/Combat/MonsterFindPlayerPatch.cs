namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Data;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterFindPlayerPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterFindPlayerPatch()
    {
        Target = RequireMethod<Monster>("findPlayer");
        Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer? __result)
    {
        try
        {
            var location = Game1.currentLocation;
            Farmer? target = null;
            if (__instance is GreenSlime slime && ModDataIO.ReadFrom<bool>(slime, "Piped"))
            {
                var aggroee = slime.GetClosestCharacter(out _,
                    location.characters.OfType<Monster>().Where(m => !m.IsSlime()));
                if (aggroee is not null)
                {
                    var fakeFarmerId = slime.GetHashCode();
                    if (ModEntry.HostState.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer))
                    {
                        fakeFarmer.Position = aggroee.Position;
                        target = fakeFarmer;
                        ModDataIO.WriteTo(slime, "Aggroee", aggroee.GetHashCode().ToString());
                    }
                }
            }
            else if (ModDataIO.ReadFrom<bool>(__instance, "Aggroed"))
            {
                var fakeFarmerId = __instance.GetHashCode();
                if (ModEntry.HostState.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer) &&
                    location.TryGetCharacterByHash<GreenSlime>(ModDataIO.ReadFrom<int>(__instance, "Aggroer"),
                        out var aggroer))
                {
                    fakeFarmer.Position = aggroer.Position;
                    target = fakeFarmer;
                }
            }

            __result = target ?? (Context.IsMultiplayer
                ? __instance.GetClosestFarmer(out _,
                    predicate: f => !ModEntry.HostState.FakeFarmers.ContainsKey(f.UniqueMultiplayerID) &&
                                    !ModEntry.HostState.PoachersInAmbush.Contains(f.UniqueMultiplayerID))
                : Game1.player);
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