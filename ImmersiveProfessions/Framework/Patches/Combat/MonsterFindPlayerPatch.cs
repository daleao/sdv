namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
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
        Prefix.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer __result)
    {
        try
        {
            var location = Game1.currentLocation;
            Farmer target = null;
            if (__instance is GreenSlime slime && slime.ReadDataAs<bool>("Piped"))
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
                        slime.WriteData("Aggroee", aggroee.GetHashCode().ToString());
                    }
                }
            }
            else if (__instance.ReadDataAs<bool>("Aggroed"))
            {
                var fakeFarmerId = __instance.GetHashCode();
                if (ModEntry.HostState.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer) &&
                    location.FindCharacterByHash<GreenSlime>(__instance.ReadDataAs<int>("Aggroer"), out var aggroer))
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