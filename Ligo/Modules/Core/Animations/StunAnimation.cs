namespace DaLion.Ligo.Modules.Core.Animations;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a stunned <see cref="Monster"/>.</summary>
public class StunAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="StunAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public StunAnimation(Monster monster, int duration)
        : base(
            $"{Manifest.UniqueID}/StunAnimation",
            new Rectangle(0, 0, 64, 64),
            50f,
            4,
            duration / 250,
            Vector2.Zero,
            false,
            Game1.random.NextBool())
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
    }

    internal static ConditionalWeakTable<Monster, StunAnimation> StunAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (!result)
        {
            var offset = new Vector2(0f, -monster.Sprite.SpriteHeight - 16f);
            switch (monster)
            {
                case Bat bat:
                    if (bat.cursedDoll.Value)
                    {
                        offset.Y += (8f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds /
                                                          (Math.PI * 60.0))) - 32f;
                        if (bat.Name == "Bat")
                        {
                            offset.X += 16f;
                        }
                        else if (bat.Name.Contains("Magma"))
                        {
                            offset.Y += 16f;
                        }
                    }

                    break;

                case BigSlime:
                    offset.X += 24f;
                    offset.Y -= 16f;
                    break;

                case BlueSquid blueSquid:
                    offset.Y += blueSquid.squidYOffset;
                    break;

                case Bug bug:
                    var sin = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.Milliseconds / 1000f *
                                               (Math.PI * 2.0)) * 10.0);
                    if (bug.FacingDirection % 2 == 0)
                    {
                        offset.X += sin;
                    }
                    else
                    {
                        offset.Y += sin;
                    }

                    offset.Y -= 64f;
                    break;

                case DinoMonster dino:
                    offset.X += dino.FacingDirection == (int)FacingDirection.Right ? 48f : dino.FacingDirection == (int)FacingDirection.Left ? 0f : 24f;
                    offset.Y += dino.FacingDirection == (int)FacingDirection.Up ? -16f : 16f;
                    break;

                case DustSpirit dustSpirit:
                    offset.Y += dustSpirit.yJumpOffset + 16f;
                    break;

                case DwarvishSentry:
                    offset.Y += (int)(Math.Sin(time.TotalGameTime.Milliseconds / 2000f * (Math.PI * 2.0)) * 7.0) - 40f;
                    break;

                case Fly:
                case MetalHead:
                case RockCrab { Name: "False Magma Cap" }:
                    offset.Y -= 16f;
                    break;

                case Ghost:
                    offset.Y += (int)(Math.Sin(time.TotalGameTime.Milliseconds / 1000f * (Math.PI * 2.0)) * 20.0) - 32f;
                    break;

                case LavaLurk lurk:
                    if (lurk.currentState.Value is LavaLurk.State.Emerged or LavaLurk.State.Firing)
                    {
                        offset.Y -= 32f;
                    }
                    else if (lurk.currentState.Value is LavaLurk.State.Lurking)
                    {
                        offset.Y -= 16f;
                    }

                    break;

                case RockGolem:
                    offset.Y -= 32f;
                    break;

                case Serpent:
                    offset.X += 32f;
                    offset.Y += 24f;
                    break;

                case ShadowBrute or ShadowShaman or Shooter or Skeleton:
                    offset.Y -= 48f;
                    break;
            }

            this.Position = offset;
        }
        else
        {
            StunAnimationByMonster.Remove(monster);
        }

        return result;
    }
}
