namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterFindPlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterFindPlayerPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterFindPlayerPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>("findPlayer");
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("Esca.FarmTypeManager")]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer? __result)
    {
        if (__instance.currentLocation is not { } location)
        {
            return true; // run original logic
        }

        if ((Game1.ticks + __instance.currentLocation.characters.IndexOf(__instance)) % 30 != 0)
        {
            __result = __instance.Get_Target();
            return false; // don't run original logic
        }

        try
        {
            Farmer? target = null;

            var closestMusk = __instance.GetClosest(
                location.Get_Musks(),
                musk => musk.FakeFarmer.Tile,
                out var distance,
                musk => !ReferenceEquals(musk.AttachedMonster, __instance));
            if (closestMusk is not null && distance < 10)
            {
                __result = closestMusk.FakeFarmer;
                __instance.Set_Target(__result);
                return false; // don't run original logic
            }

            if (__instance is GreenSlime slime && slime.Get_Piped() is { } piped)
            {
                Vector2? targetPos = null;
                var aggroee = slime.GetClosestCharacter(
                    location.characters.OfType<Monster>(),
                    m => !m.IsSlime() && m is not Spiker &&
                         (m is not Duggy duggy || duggy.Sprite.CurrentFrame is > 0 and < 10) &&
                         m is not LavaLurk &&
                         slime.IsCharacterWithinThreshold(m));
                if (aggroee is not null)
                {
                    targetPos = aggroee.Position;
                    piped.FakeFarmer.IsEnemy = true;
                }
                else if (State.AlliedSlimes.Contains(piped) && slime.Get_HasInventorySlots())
                {
                    var approximatePosition =
                        Reflector.GetUnboundMethodDelegate<Func<Debris, Vector2>>(
                            typeof(Debris),
                            "approximatePosition");
                    var closest = slime.GetClosest(
                        location.debris,
                        d => approximatePosition(d),
                        out _,
                        d => d.itemId.Value is not null);
                    if (closest is not null)
                    {
                        targetPos = approximatePosition(closest);
                        piped.FakeFarmer.IsEnemy = false;
                    }
                }

                if (targetPos is null)
                {
                    var mapWidth = location.Map.Layers[0].LayerWidth;
                    var mapHeight = location.Map.Layers[0].LayerHeight;
                    targetPos = slime.GetClosestTile(piped.Piper.Tile.GetEightNeighbors(mapWidth, mapHeight)) *
                                 Game1.tileSize;
                    piped.FakeFarmer.IsEnemy = false;
                }

                piped.FakeFarmer.Position = targetPos.Value;
                target = piped.FakeFarmer;
                __result = target;
                __instance.Set_Target(target);
                return false; // don't run original logic
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
                ? __instance.GetClosestFarmer(predicate: f => f is not FakeFarmer && !f.IsAmbushing())
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
