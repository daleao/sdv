namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;
using DaLion.Professions.Framework.Integrations;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(
            nameof(GreenSlime.update), [typeof(GameTime), typeof(GameLocation)]);
    }

    #region harmony patches

    /// <summary>Patch for Slimes to damage monsters around Piper and collect items.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeUpdatePostfix(GreenSlime __instance, ref int ___readyToJump, GameTime time)
    {
        var location = __instance.currentLocation;
        if (__instance.Get_Piped() is not { } piped ||
            !ReferenceEquals(location, piped.Piper.currentLocation))
        {
            if (__instance.currentLocation.DoesAnyPlayerHereHaveProfession(Profession.Piper))
            {
                ___readyToJump = -1;
            }

            return;
        }

        if (time.TotalGameTime.Ticks % 60 == 0 &&
            !Utility.isOnScreen(__instance.TilePoint, 4 * Game1.tileSize, location))
        {
            piped.WarpToPiper();
        }

        if (!piped.FakeFarmer.IsAttachedToEnemy)
        {
            ___readyToJump = -1;
        }
        else if (piped.FakeFarmer.AttachedEnemy.IsFloating() && __instance.Scale < 1.8f && ___readyToJump == -1)
        {
            ___readyToJump = 800;
        }

        // do looting behavior
        if (piped.Hat is not null)
        {
            var approximatePosition =
                Reflector.GetUnboundMethodDelegate<Func<Debris, Vector2>>(
                    typeof(Debris),
                    "approximatePosition");
            for (var i = location.debris.Count - 1; i >= 0; i--)
            {
                var debris = location.debris[i];
                if (debris.itemId is null)
                {
                    continue;
                }

                var (x, y) = approximatePosition(debris) / Game1.tileSize;
                if (!__instance.Tile.X.Approx(x, 0.9f) || !__instance.Tile.Y.Approx(y, 0.9f) ||
                    !piped.TryCollectDebris(debris))
                {
                    continue;
                }

                location.debris.RemoveAt(i);
                if (!piped.HasEmptyInventorySlots)
                {
                    Game1.addHUDMessage(new HUDMessage(I18n.Piper_Slime_BagFull()));
                }
            }

            return;
        }

        // do and receive damage
        foreach (var character in location.characters)
        {
            if (character is not Monster { IsMonster: true } monster
                || (monster.IsFloating() && !(__instance.Scale >= 1.8f || __instance.IsJumping()))
                || monster.IsSlime())
            {
                continue;
            }

            var monsterBox = monster.GetBoundingBox();
            if (!monsterBox.Intersects(__instance.GetBoundingBox()))
            {
                continue;
            }

            // damage monster
            var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : Utility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            if (monster.CanBeDamaged())
            {
                var randomizedDamage = __instance.DamageToFarmer +
                                       Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4);
                var mitigatedDamage = (CombatIntegration.Instance?.IsLoaded ?? false) &&
                                      CombatIntegration.Instance.ModApi.GetConfig().HyperbolicMitigationFormula
                    ? (int)(randomizedDamage * (10f / (10f + monster.resilience.Value)))
                    : randomizedDamage - monster.resilience.Value;
                var damageToMonster = Math.Max(1, mitigatedDamage);
                monster.takeDamage(damageToMonster, (int)xTrajectory, (int)yTrajectory, false, 1d, "slime");
                location.debris.Add(new Debris(
                    damageToMonster,
                    new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
                    new Color(255, 130, 0),
                    1f,
                    monster));
                if (!monster.IsSlime() && piped.Piper.HasProfession(Profession.Piper, true) &&
                    Game1.random.NextBool(0.3))
                {
                    ApplyColoredDebuff(__instance, monster, piped);
                }

                // aggro monsters
                if (monster.Get_Taunter() is null)
                {
                    monster.Set_Taunter(__instance);
                }

                var fakeFarmer = monster.Get_TauntFakeFarmer();
                if (fakeFarmer is not null)
                {
                    fakeFarmer.Position = __instance.Position;
                }

                monster.setInvincibleCountdown(piped.Piper.Get_IsLimitBreaking().Value ? 300 : 450);
            }

            if (__instance.CanBeDamaged() && (!monster.IsBlinded() || Game1.random.NextBool()))
            {
                // get damaged by monster
                var randomizedDamage = monster.DamageToFarmer +
                                   Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4);
                var mitigatedDamage = (CombatIntegration.Instance?.IsLoaded ?? false) &&
                                  CombatIntegration.Instance.ModApi.GetConfig().HyperbolicMitigationFormula
                    ? (int)(randomizedDamage * (10f / (10f + __instance.resilience.Value)))
                    : randomizedDamage - __instance.resilience.Value;
                var damageToSlime = Math.Max(1, mitigatedDamage);
                __instance.takeDamage(damageToSlime, (int)-xTrajectory, (int)-yTrajectory, false, 1d, "slime");
                if (__instance.IsTigerSlime())
                {
                    monster.takeDamage(damageToSlime / 2, 0, 0, false, 1d, "hitEnemy");
                }

                if (__instance.Health > 0)
                {
                    __instance.setInvincibleCountdown(450);
                    continue;
                }

                piped.BeginRespawn();
                break;
            }
        }

        if (time.TotalGameTime.Ticks % Game1.random.Next(60, 120) == 0)
        {
            var color = __instance.IsTigerSlime() ? Color.OrangeRed : __instance.color.Value;
            var sprite = new TemporaryAnimatedSprite("LooseSprites/Cursors", new Rectangle(359, 1437, 14, 14), Vector2.Zero, flipped: false, 0.01f, color)
            {
                alpha = (float)((Game1.random.NextDouble() / 3.0) + 0.5),
                xPeriodic = true,
                xPeriodicLoopTime = Game1.random.Next(2000, 3000),
                xPeriodicRange = Game1.random.Next(-32, 32),
                motion = new Vector2(0f, -1f),
                rotationChange = (float)(Math.PI / Game1.random.Next(32, 64)),
                positionFollowsAttachedCharacter = true,
                attachedCharacter = __instance,
                layerDepth = 1f,
                scaleChange = 0.04f,
                scaleChangeChange = -0.0008f,
                scale = (float)(2.0 + Game1.random.NextDouble()),
            };

            Game1.Multiplayer.broadcastSprites(location, sprite);
        }

        if (!piped.Piper.HasProfession(Profession.Piper, true) || time.TotalGameTime.TotalSeconds % 5 != 0)
        {
            return;
        }

        // do white slime healing
        var whiteRange = new ColorRange(
            [230, 255],
            [230, 255],
            [230, 255]);
        if (!whiteRange.Contains(__instance.color.Value))
        {
            return;
        }

        foreach (var farmer in location.farmers)
        {
            if (__instance.TileDistanceToPlayer(farmer) < 3)
            {
                HealingSlimeOneSecondUpdateTickedEvent.FarmersInRange.TryAdd(farmer, 0);
            }
            else
            {
                HealingSlimeOneSecondUpdateTickedEvent.FarmersInRange.Remove(farmer);
            }
        }
    }

    #endregion harmony patches

    private static void ApplyColoredDebuff(GreenSlime slime, Monster monster, PipedSlime piped)
    {
        if (slime.Name == "Gold Slime" || slime.prismatic.Value)
        {
            return;
        }

        var greenRange = new ColorRange(
            [22, 127],
            [200, 255],
            [0, 55]);
        if (greenRange.Contains(slime.color.Value))
        {
            // simulated Slimed debuff
            monster.Slow(5123 + (Game1.random.Next(-2, 3) * 456), 1f / 3f);
            monster.startGlowing(Color.LimeGreen, false, 0.05f);
            return;
        }

        var blueRange = new ColorRange(
            [22, 180],
            [170, 255],
            [200, 255]);
        if (blueRange.Contains(slime.color.Value))
        {
            monster.Chill(5123 + (Game1.random.Next(-2, 3) * 456), 1f / 3f);
            return;
        }

        var redRange = new ColorRange(
            [200, 255],
            [0, 55],
            [22, 127]);
        var purpleRange = new ColorRange(
            [138, 158],
            [23, 63],
            [206, 246]);
        if (redRange.Contains(slime.color.Value) || purpleRange.Contains(slime.color.Value))
        {
            monster.Burn(piped.Piper, 5123 + (Game1.random.Next(-2, 3) * 456));
            return;
        }

        var blackRange = new ColorRange(
            [0, 50],
            [0, 55],
            [0, 50]);
        if (blackRange.Contains(slime.color.Value) && Game1.random.NextBool(0.05))
        {
            monster.Blind(5123 + (Game1.random.Next(-2, 3) * 456));
        }
    }
}
