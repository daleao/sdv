namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>Randomizes the stats of the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void RandomizeStats(this Monster monster)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());
        var mean = 1d - (Game1.player.DailyLuck * 3d);

        var g = Math.Max(r.NextGaussian(mean, 0.5), 0.25);
        monster.MaxHealth = (int)Math.Round(monster.MaxHealth * g);

        g = Math.Max(r.NextGaussian(mean, 0.5), 0.5);
        monster.DamageToFarmer = (int)Math.Round(monster.DamageToFarmer * g);

        g = Math.Max(r.NextGaussian(mean, 0.5), 0.5);
        monster.resilience.Value = (int)Math.Round(monster.resilience.Value * g);

        var addedSpeed = r.NextDouble() > 0.5 + (Game1.player.DailyLuck * 2d) ? 1 :
            r.NextDouble() < 0.5 - (Game1.player.DailyLuck * 2d) ? -1 : 0;
        monster.speed = Math.Max(monster.speed + addedSpeed, 1);

        monster.durationOfRandomMovements.Value =
            (int)(monster.durationOfRandomMovements.Value * (0.5d + r.NextDouble()));
        monster.moveTowardPlayerThreshold.Value =
            Math.Max(monster.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
    }

    /// <summary>Causes bleeding on <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="bleeder">The <see cref="Farmer"/> who caused the bleeding.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the bleeding effect (how many stacks).</param>
    internal static void Bleed(this Monster monster, Farmer bleeder, int duration = 30000, int intensity = 1)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        if (monster is Skeleton or Ghost or DwarvishSentry or RockGolem or Bat { hauntedSkull.Value: true } or Bat { cursedDoll.Value: true })
        {
            return;
        }

        monster.Set_Bleeder(bleeder);
        monster.Get_BleedTimer().Value = duration;
        monster.Get_BleedStacks().Value = Math.Min(monster.Get_BleedStacks().Value + intensity, 5);
        //monster.startGlowing(Color.Maroon, true, 0.05f);
        BleedAnimation.BleedAnimationByMonster.AddOrUpdate(monster, new BleedAnimation(monster, duration));
    }

    /// <summary>Removes bleeding from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void StopBleeding(this Monster monster)
    {
        monster.Set_Bleeder(null);
        monster.Get_BleedTimer().Value = -1;
        monster.Get_BleedStacks().Value = 0;
        BleedAnimation.BleedAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is bleeding.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero bleeding stacks, otherwise <see langword="false"/>.</returns>
    internal static bool IsBleeding(this Monster monster)
    {
        return monster.Get_BleedStacks().Value > 0;
    }

    /// <summary>Burns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="burner">The <see cref="Farmer"/> who inflicted the burn.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Burn(this Monster monster, Farmer burner, int duration = 15000)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        switch (monster)
        {
            case LavaLurk or Bat { magmaSprite.Value: true }:
                return;
            case Bug or Fly:
                monster.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
                    30,
                    monster.GetBoundingBox().Center.ToVector2(),
                    Color.White,
                    4,
                    Game1.random.NextBool(),
                    50f,
                    2)
                {
                    positionFollowsAttachedCharacter = true,
                    attachedCharacter = monster,
                    layerDepth = 999999f,
                });

                monster.Die(burner);
                return;
        }

        monster.Set_Burner(burner);
        monster.Get_BurnTimer().Value = duration;
        //monster.startGlowing(Color.OrangeRed, true, 0.05f);
        BurnAnimation.BurnAnimationByMonster.AddOrUpdate(monster, new BurnAnimation(monster, duration));
    }

    /// <summary>Removes burn from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void CureBurn(this Monster monster)
    {
        monster.Set_Burner(null);
        monster.Get_BurnTimer().Value = -1;
        BurnAnimation.BurnAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is burning.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero burn timer, otherwise <see langword="false"/>.</returns>
    internal static bool IsBurning(this Monster monster)
    {
        return monster.Get_BurnTimer().Value > 0;
    }

    /// <summary>Chills the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Chill(this Monster monster, int duration = 5000)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        if (monster.Get_Chilled())
        {
            monster.Get_Frozen().Value = true;
            monster.Get_SlowIntensity().Value = 1d;
            monster.Get_SlowTimer().Value = duration * 10;
        }
        else
        {
            monster.Get_Chilled().Value = true;
            monster.Get_SlowIntensity().Value = 0.5;
            monster.Get_SlowTimer().Value = duration;
        }

        monster.startGlowing(Color.PowderBlue, true, 0.05f);
    }

    /// <summary>Removes chilled status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void WarmUp(this Monster monster)
    {
        monster.Get_Chilled().Value = false;
        monster.RemoveSlow();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is chilled.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns>The <paramref name="monster"/>'s chilled flag.</returns>
    internal static bool IsChilled(this Monster monster)
    {
        return monster.Get_Chilled().Value;
    }

    /// <summary>Fears the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Fear(this Monster monster, int duration)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        monster.Get_FearTimer().Value = duration;
    }

    /// <summary>Removes fear from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void CureFear(this Monster monster)
    {
        monster.Get_FearTimer().Value = -1;
    }

    /// <summary>Checks whether the <paramref name="monster"/> is feared.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero fear timer, otherwise <see langword="false"/>.</returns>
    internal static bool IsFeared(this Monster monster)
    {
        return monster.Get_FearTimer().Value > 0;
    }

    /// <summary>Freezes the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Freeze(this Monster monster, int duration = 30000)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        monster.Chill(duration);
        monster.Chill(duration);
    }

    /// <summary>Removes frozen status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void Defrost(this Monster monster)
    {
        monster.Get_Frozen().Value = false;
        monster.WarmUp();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is frozen.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero freeze stacks, otherwise <see langword="false"/>.</returns>
    internal static bool IsFrozen(this Monster monster)
    {
        return monster.Get_Frozen().Value;
    }

    /// <summary>Poisons the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="poisoner">The <see cref="Farmer"/> who inflicted the poison.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect (how many stacks).</param>
    internal static void Poison(this Monster monster, Farmer poisoner, int duration = 15000, int intensity = 1)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        monster.Set_Poisoner(poisoner);
        monster.Get_PoisonTimer().Value = duration;
        monster.Get_PoisonStacks().Value = Math.Min(monster.Get_PoisonStacks().Value + intensity, 3);
        //monster.startGlowing(Color.LimeGreen, true, 0.05f);
        PoisonAnimation.PoisonAnimationByMonster.AddOrUpdate(monster, new PoisonAnimation(monster, duration));
    }

    /// <summary>Removes poison from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void CurePoison(this Monster monster)
    {
        monster.Set_Poisoner(null);
        monster.Get_PoisonTimer().Value = -1;
        monster.Get_PoisonStacks().Value = 0;
        PoisonAnimation.PoisonAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is poisoned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero poison stacks, otherwise <see langword="false"/>.</returns>
    internal static bool IsPoisoned(this Monster monster)
    {
        return monster.Get_PoisonStacks().Value > 0;
    }

    /// <summary>Slows the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect.</param>
    internal static void Slow(this Monster monster, int duration, double intensity = 0.5)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        monster.Get_SlowTimer().Value = duration;
        monster.Get_SlowIntensity().Value = intensity;
        SlowAnimation.SlowAnimationByMonster.AddOrUpdate(monster, new SlowAnimation(monster, duration));
    }

    /// <summary>Removes slow from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void RemoveSlow(this Monster monster)
    {
        monster.Get_SlowTimer().Value = -1;
        monster.Get_SlowIntensity().Value = 0;
        monster.Get_Chilled().Value = false;
        monster.Get_Frozen().Value = false;
        SlowAnimation.SlowAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is slowed.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero slow timer, otherwise <see langword="false"/>.</returns>
    internal static bool IsSlowed(this Monster monster)
    {
        return monster.Get_SlowTimer().Value > 0;
    }

    /// <summary>Stuns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Stun(this Monster monster, int duration)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return;
        }

        monster.stunTime = duration;
        StunAnimation.StunAnimationByMonster.AddOrUpdate(monster, new StunAnimation(monster, duration));
    }

    /// <summary>Checks whether the <paramref name="monster"/> is stunned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero stun timer, otherwise <see langword="false"/>.</returns>
    internal static bool IsStunned(this Monster monster)
    {
        return monster.stunTime > 0;
    }

    internal static Vector2 GetOverheadOffset(this Monster monster, GameTime time)
    {
        var position = new Vector2(0f, -monster.Sprite.SpriteHeight - 16f);
        switch (monster)
        {
            case Bat bat:
                if (bat.cursedDoll.Value)
                {
                    position.Y += (8f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (Math.PI * 60.0))) - 32f;
                    if (bat.Name == "Bat")
                    {
                        position.X += 16f;
                    }
                    else if (bat.Name.Contains("Magma"))
                    {
                        position.Y += 16f;
                    }
                }

                break;

            case BigSlime:
                position.X += 24f;
                position.Y -= 16f;
                break;

            case BlueSquid blueSquid:
                position.Y += blueSquid.squidYOffset;
                break;

            case Bug bug:
                var sin = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.Milliseconds / 1000f *
                                           (Math.PI * 2.0)) * 10.0);
                if (bug.FacingDirection % 2 == 0)
                {
                    position.X += sin;
                }
                else
                {
                    position.Y += sin;
                }

                position.Y -= 64f;
                break;

            case DinoMonster dino:
                position.X += dino.FacingDirection == (int)FacingDirection.Right ? 48f :
                    dino.FacingDirection == (int)FacingDirection.Left ? 0f : 24f;
                position.Y += dino.FacingDirection == (int)FacingDirection.Up ? -16f : 16f;
                break;

            case DustSpirit dustSpirit:
                position.Y += dustSpirit.yJumpOffset + 16f;
                break;

            case DwarvishSentry:
                position.Y += (int)(Math.Sin(time.TotalGameTime.Milliseconds / 2000f * (Math.PI * 2.0)) * 7.0) - 40f;
                break;

            case Fly:
            case MetalHead:
            case RockCrab { Name: "False Magma Cap" }:
                position.Y -= 16f;
                break;

            case Ghost:
                position.Y += (int)(Math.Sin(time.TotalGameTime.Milliseconds / 1000f * (Math.PI * 2.0)) * 20.0) - 32f;
                break;

            case LavaLurk lurk:
                if (lurk.currentState.Value is LavaLurk.State.Emerged or LavaLurk.State.Firing)
                {
                    position.Y -= 32f;
                }
                else if (lurk.currentState.Value is LavaLurk.State.Lurking)
                {
                    position.Y -= 16f;
                }

                break;

            case Mummy:
            case RockGolem:
                position.Y -= 32f;
                break;

            case Serpent:
                position.X += 32f;
                position.Y += 24f;
                break;

            case ShadowBrute or ShadowShaman or Shooter or Skeleton:
                position.Y -= 48f;
                break;
        }

        return position;
    }
}
