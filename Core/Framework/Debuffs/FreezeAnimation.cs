namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Core.Framework.Events;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a burning <see cref="Monster"/>.</summary>
public class FreezeAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="FreezeAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="offset">A <see cref="Vector2"/> offset to the animation's position.</param>
    public FreezeAnimation(Monster monster, int duration, Vector2? offset = null)
        : base(
            "LooseSprites\\Cursors2",
            new Rectangle(118, 227, 16, 13),
            monster.getStandingPosition() + new Vector2(-32f, -21f) + (offset ?? Vector2.Zero),
            Game1.random.NextBool(),
            0f,
            Color.White)
    {
        this.attachedCharacter = monster;
        this.animationLength = 1;
        this.layerDepth = (monster.StandingPixel.Y + 1) / 10000f;
        this.interval = duration;
        this.scale = 4f;
        EventManager.Enable<FreezeAnimationUpdateTickedEvent>();
        EventManager.Enable<FreezeAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, List<FreezeAnimation>> FreezeAnimationsByMonster { get; } = [];

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (!result && monster.Health > 0 && monster.IsFrozen())
        {
            return result;
        }

        FreezeAnimationsByMonster.Remove(monster);
        return result;
    }
}
