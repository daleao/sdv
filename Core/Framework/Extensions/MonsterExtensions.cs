namespace DaLion.Core.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Core.Framework.Debuffs;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
public static class MonsterExtensions
{
    /// <summary>Causes bleeding on the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="stacks"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="bleeder">The <see cref="Farmer"/> who caused the bleeding.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="stacks">The intensity of the bleeding effect (how many stacks).</param>
    /// <param name="maxStacks">The max number of allowed stacks.</param>
    public static void Bleed(
        this Monster monster,
        Farmer bleeder,
        int duration = 30000,
        int stacks = 1,
        int maxStacks = 5)
    {
        if (monster is Skeleton or Ghost or DwarvishSentry or RockGolem or Bat { hauntedSkull.Value: true }
            or Bat { cursedDoll.Value: true })
        {
            return;
        }

        monster.SetOrIncrement_Bleeding(duration, stacks, bleeder, maxStacks);
        monster.startGlowing(Color.Maroon, true, 0.05f);
        BleedAnimation.BleedAnimationByMonster.AddOrUpdate(monster, new BleedAnimation(monster, duration));
    }

    /// <summary>Removes bleeding from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Unbleed(this Monster monster)
    {
        monster.Set_Bleeding(-1, 0, null);
        monster.stopGlowing();
        BleedAnimation.BleedAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is bleeding.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero bleeding stacks, otherwise <see langword="false"/>.</returns>
    public static bool IsBleeding(this Monster monster)
    {
        return monster.Get_BleedStacks().Value > 0;
    }

    /// <summary>Burns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="burner">The <see cref="Farmer"/> who inflicted the burn.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void Burn(this Monster monster, Farmer burner, int duration = 15000)
    {
        if (monster is LavaLurk or Bat { magmaSprite.Value: true })
        {
            return;
        }

        if (monster.IsFrozen())
        {
            monster.Defrost();
        }
        else if (monster.IsChilled())
        {
            monster.Unchill();
        }

        monster.Set_Burnt(duration, burner);
        monster.startGlowing(Color.Yellow, true, 0.05f);
        monster.jitteriness.Value *= 2;
        switch (monster)
        {
            case Serpent serpent when serpent.IsRoyalSerpent():
                var burnList = new List<BurnAnimation>();
                for (var i = serpent.segments.Count - 1; i >= 0; i--)
                {
                    burnList.Add(new BurnAnimation(serpent, duration, i));
                }

                burnList.Add(new(monster, duration));
                BurnAnimation.BurnAnimationsByMonster.AddOrUpdate(monster, burnList);
                break;

            default:
                BurnAnimation.BurnAnimationsByMonster.AddOrUpdate(
                    monster,
                    [new(monster, duration)]);
                break;
        }
    }

    /// <summary>Removes burn from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Unburn(this Monster monster)
    {
        monster.jitteriness.Value /= 2;
        monster.Set_Burnt(-1, null);
        monster.stopGlowing();
        BurnAnimation.BurnAnimationsByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is burning.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero burn timer, otherwise <see langword="false"/>.</returns>
    public static bool IsBurning(this Monster monster)
    {
        return monster.Get_BurnTimer().Value > 0;
    }

    /// <summary>Chills the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect.</param>
    /// <param name="freezeThreshold">The required slow intensity total for the target to be considered frozen.</param>
    /// <param name="playSoundEffect">Whether to play the chill sound effect.</param>
    public static void Chill(
        this Monster monster,
        int duration = 5000,
        float intensity = 0.5f,
        float freezeThreshold = 1f,
        bool playSoundEffect = true)
    {
        if (monster is Ghost or Skeleton { isMage.Value: true })
        {
            return;
        }

        if (monster.IsBurning())
        {
            monster.Unburn();
        }

        if (monster.IsChilled())
        {
            monster.SetOrIncrement_Slowed(duration, intensity);
            if (monster.Get_SlowIntensity().Value >= freezeThreshold)
            {
                monster.Get_Frozen().Value = true;
                monster.Get_SlowTimer().Value *= 5;
                switch (monster)
                {
                    case BigSlime:
                        FreezeAnimation.FreezeAnimationsByMonster.AddOrUpdate(
                            monster,
                            [
                                new FreezeAnimation(monster, duration, new Vector2(32f, 0f)),
                                new FreezeAnimation(monster, duration, new Vector2(-32f, 0f)),
                            ]);
                        break;

                    case DinoMonster:
                        var facingDirection = (FacingDirection)monster.FacingDirection;
                        if (facingDirection.IsHorizontal())
                        {
                            FreezeAnimation.FreezeAnimationsByMonster.AddOrUpdate(
                                monster,
                                [
                                    new FreezeAnimation(monster, duration, new Vector2(32f, 8f)),
                                    new FreezeAnimation(monster, duration, new Vector2(-32f, 8f)),
                                ]);
                        }
                        else
                        {
                            FreezeAnimation.FreezeAnimationsByMonster.AddOrUpdate(
                                monster,
                                [
                                    new FreezeAnimation(monster, duration, new Vector2(32f, 20f)),
                                    new FreezeAnimation(monster, duration, new Vector2(-32f, 20f)),
                                ]);
                        }

                        break;

                    case Serpent serpent when serpent.IsRoyalSerpent():
                        var freezeList = new List<FreezeAnimation>();
                        for (var i = serpent.segments.Count - 1; i >= 0; i--)
                        {
                            var (x, y, _) = serpent.segments[i];
                            var offset = new Vector2(x + 64f, y + 64f) - serpent.getStandingPosition();
                            freezeList.Add(new FreezeAnimation(monster, duration, offset));
                        }

                        freezeList.Add(new FreezeAnimation(monster, duration));
                        FreezeAnimation.FreezeAnimationsByMonster.AddOrUpdate(monster, freezeList);
                        break;

                    default:
                        FreezeAnimation.FreezeAnimationsByMonster.AddOrUpdate(
                            monster,
                            [new FreezeAnimation(monster, duration)]);
                        break;
                }

                SoundBox.Freeze.PlayAll(monster.currentLocation, monster.Tile, Game1.random.Next(-3, 4) * 100);
            }
        }
        else
        {
            monster.Get_Chilled().Value = true;
            monster.SetOrIncrement_Slowed(duration, 0.5f);
            if (playSoundEffect)
            {
                SoundBox.Chill.PlayAll(monster.currentLocation, monster.Tile, Game1.random.Next(-3, 4) * 100);
            }
        }

        monster.startGlowing(Color.PowderBlue, true, 0.05f);
    }

    /// <summary>Removes chilled status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Unchill(this Monster monster)
    {
        monster.Unslow();
        monster.Get_Chilled().Value = false;
        monster.stopGlowing();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is chilled.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns>The <paramref name="monster"/>'s chilled flag.</returns>
    public static bool IsChilled(this Monster monster)
    {
        return monster.Get_Chilled().Value;
    }

    /// <summary>Fears the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void Fear(this Monster monster, int duration = 5000)
    {
        monster.Get_FearTimer().Value = duration;
    }

    /// <summary>Removes fear from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Unfear(this Monster monster)
    {
        monster.Get_FearTimer().Value = -1;
    }

    /// <summary>Checks whether the <paramref name="monster"/> is feared.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero fear timer, otherwise <see langword="false"/>.</returns>
    public static bool IsFeared(this Monster monster)
    {
        return monster.Get_FearTimer().Value > 0;
    }

    /// <summary>Freezes the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void Freeze(this Monster monster, int duration = 30000)
    {
        if (!monster.IsChilled())
        {
            monster.Chill();
        }

        monster.Chill(duration);
    }

    /// <summary>Removes frozen status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Defrost(this Monster monster)
    {
        monster.Unchill();
        monster.Get_Frozen().Value = false;
        FreezeAnimation.FreezeAnimationsByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is frozen.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero freeze stacks, otherwise <see langword="false"/>.</returns>
    public static bool IsFrozen(this Monster monster)
    {
        return monster.Get_Frozen().Value;
    }

    /// <summary>Poisons the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="stacks"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="poisoner">The <see cref="Farmer"/> who inflicted the poison.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="stacks">The intensity of the poison effect (how many stacks).</param>
    /// <param name="maxStacks">This number of stacks will immediately kill the monster.</param>
    public static void Poison(
        this Monster monster,
        Farmer poisoner,
        int duration = 15000,
        int stacks = 1,
        int maxStacks = int.MaxValue)
    {
        if (monster is Ghost)
        {
            return;
        }

        // if max stacks reached; i.e., failed to increment
        if (!monster.Increment_Poisoned(duration, stacks, poisoner, maxStacks))
        {
            PoisonAnimation.PoisonAnimationByMonster.Remove(monster);
            return;
        }

        monster.startGlowing(Color.Purple, true, 0.05f);
        PoisonAnimation.PoisonAnimationByMonster.AddOrUpdate(monster, new PoisonAnimation(monster, duration));
    }

    /// <summary>Removes poison from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Detox(this Monster monster)
    {
        monster.Set_Poisoned(-1, 0, null);
        monster.stopGlowing();
        PoisonAnimation.PoisonAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is poisoned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero poison stacks, otherwise <see langword="false"/>.</returns>
    public static bool IsPoisoned(this Monster monster)
    {
        return monster.Get_PoisonStacks().Value > 0;
    }

    /// <summary>Slows the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect.</param>
    public static void Slow(this Monster monster, int duration = 5000, float intensity = 0.5f)
    {
        monster.Set_Slowed(duration, intensity);
        SlowAnimation.SlowAnimationByMonster.AddOrUpdate(monster, new SlowAnimation(monster, duration));
    }

    /// <summary>Removes slow from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public static void Unslow(this Monster monster)
    {
        monster.Set_Slowed(-1, 0f);
        SlowAnimation.SlowAnimationByMonster.Remove(monster);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is slowed.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero slow timer, otherwise <see langword="false"/>.</returns>
    public static bool IsSlowed(this Monster monster)
    {
        return monster.Get_SlowTimer().Value > 0;
    }

    /// <summary>Stuns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void Stun(this Monster monster, int duration = 5000)
    {
        monster.stunTime.Value = duration;
        StunAnimation.StunAnimationByMonster.AddOrUpdate(monster, new StunAnimation(monster, duration));
    }

    /// <summary>Checks whether the <paramref name="monster"/> is stunned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero stun timer, otherwise <see langword="false"/>.</returns>
    public static bool IsStunned(this Monster monster)
    {
        return monster.stunTime.Value > 0;
    }

    /// <summary>Checks the pixel offset to clear the <paramref name="monster"/>s head.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns>The X and Y offsets as a <see cref="Vector2"/>.</returns>
    public static Vector2 GetOverheadOffset(this Monster monster)
    {
        var position = new Vector2(0f, -monster.Sprite.SpriteHeight - 16f);
        switch (monster)
        {
            case Bat bat:
                if (bat.cursedDoll.Value)
                {
                    position.Y +=
                        (8f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds /
                                              (Math.PI * 60.0))) - 32f;
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
                position.Y +=
                    (int)(Math.Sin(Game1.currentGameTime.TotalGameTime.Milliseconds / 2000f * (Math.PI * 2.0)) * 7.0) -
                    40f;
                break;

            case Fly:
            case MetalHead:
            case RockCrab { Name: "False Magma Cap" }:
                position.Y -= 16f;
                break;

            case Ghost:
                position.Y +=
                    (int)(Math.Sin(Game1.currentGameTime.TotalGameTime.Milliseconds / 1000f * (Math.PI * 2.0)) * 20.0) -
                    32f;
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
                position.Y += 64f;
                break;

            case ShadowBrute or ShadowShaman or Shooter or Skeleton:
                position.Y -= 48f;
                break;
        }

        return position;
    }
}
