namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterFindPlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterFindPlayerPatcher"/> class.</summary>
    internal MonsterFindPlayerPatcher()
    {
        this.Target = this.RequireMethod<Monster>("findPlayer");
        this.Prefix!.priority = Priority.First;
        this.Prefix.before = new[] { "Esca.FarmTypeManager" };
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("Esca.FarmTypeManager")]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer? __result)
    {
        if (Game1.ticks % 30 != 0)
        {
            __result = __instance.Get_Target();
            return false; // don't run original logic
        }

        try
        {
            var location = __instance.currentLocation;
            Farmer? target = null;

            var closestMusk = __instance.GetClosest(
                location.Get_Musks(),
                musk => musk.FakeFarmer.getTileLocation(),
                out var distance,
                musk => !ReferenceEquals(musk.AttachedMonster, __instance));
            if (closestMusk is not null && distance < 10)
            {
                __result = closestMusk.FakeFarmer;
                __instance.Set_Target(__result);
                return false; // don't run original logic
            }

            if (__instance is GreenSlime slime)
            {
                var piped = slime.Get_Piped();
                if (piped is not null)
                {
                    var aggroee = slime.GetClosestCharacter(out _, location.characters
                        .OfType<Monster>()
                        .Where(m => !m.IsSlime()));
                    if (aggroee is not null)
                    {
                        piped.FakeFarmer.Position = aggroee.Position;
                        target = piped.FakeFarmer;
                    }

                    __result = target;
                    __instance.Set_Target(__result);
                    return false; // don't run original logic
                }
            }

            var taunter = __instance.Get_Taunter();
            if (taunter is not null)
            {
                var fakeFarmer = __instance.Get_TauntFakeFarmer();
                if (fakeFarmer is not null)
                {
                    fakeFarmer.Position = taunter.Position;
                    target = fakeFarmer;
                }
            }

            __result = target ?? (Context.IsMultiplayer
                ? __instance.GetClosestFarmer(out _, predicate: f => f is not FakeFarmer && !f.IsInAmbush())
                : Game1.player);
            __instance.Set_Target(__result);
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
