namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Events.Display;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above poisoned <see cref="Monster"/>.</summary>
public class PoisonAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="PoisonAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public PoisonAnimation(Monster monster, int duration)
        : base(
            "TileSheets\\Animations",
            new Rectangle(256, 1856, 64, 128),
            50f,
            6,
            duration / 300,
            monster.Position,
            false,
            Game1.random.NextBool())
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<PoisonAnimationUpdateTickedEvent>();
        EventManager.Enable<PoisonAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, PoisonAnimation> PoisonAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0)
        {
            PoisonAnimationByMonster.Remove(monster);
            return result;
        }

        var offset = monster.GetOverheadOffset(time);
        offset.Y -= 64f;
        this.Position = offset;
        return result;
    }
}
