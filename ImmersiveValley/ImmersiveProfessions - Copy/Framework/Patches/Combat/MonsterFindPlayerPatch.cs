namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.ModData;
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
            if (__instance is GreenSlime slime && ModDataIO.Read<bool>(slime, "Piped"))
            {
                var aggroee = slime.GetClosestNPC(location.characters.OfType<Monster>().Where(m => !m.IsSlime()));
                if (aggroee is not null)
                {
                    var fakeFarmerId = slime.GetHashCode();
                    if (ModEntry.Host.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer))
                    {
                        fakeFarmer.Position = aggroee.Position;
                        target = fakeFarmer;
                        ModDataIO.Write(slime, "Aggroee", aggroee.GetHashCode().ToString());
                    }
                }
            }
            else if (ModDataIO.Read<bool>(__instance, "Aggroed"))
            {
                var fakeFarmerId = __instance.GetHashCode();
                if (ModEntry.Host.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer) &&
                    location.TryGetCharacterByHash<GreenSlime>(ModDataIO.Read<int>(__instance, "Aggroer"),
                        out var aggroer))
                {
                    fakeFarmer.Position = aggroer.Position;
                    target = fakeFarmer;
                }
            }

            __result = target ?? (Context.IsMultiplayer
                ? __instance.GetClosestFarmer(predicate: f => !f.IsFakeFarmer() && !f.IsInAmbush())
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