namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a stunned <see cref="Monster"/>.</summary>
public class StunAnimation : TemporaryAnimatedSprite
{
    private Random _random = new(Guid.NewGuid().GetHashCode());

    /// <summary>Initializes a new instance of the <see cref="StunAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public StunAnimation(Monster monster, int duration)
        : base(
            $"{Manifest.UniqueID}/StunAnimation",
            new Rectangle(0, 0, 64, 64),
            50f,
            4,
            duration / 200,
            Vector2.Zero,
            false,
            Game1.random.NextBool())
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<StunAnimationRenderedWorldEvent>();
        EventManager.Enable<StunAnimationUpdateTickedEvent>();
    }

    internal static ConditionalWeakTable<Monster, StunAnimation> StunAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0)
        {
            StunAnimationByMonster.Remove(monster);
            return result;
        }

        this.Position = monster.GetOverheadOffset(time) +
                        new Vector2(this._random.Next(-1, 2), this._random.Next(-1, 2));
        return result;
    }
}
