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

/// <summary>The animation that plays above a burning <see cref="Monster"/>.</summary>
public class BurnAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="BurnAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public BurnAnimation(Monster monster, int duration)
        : base(
            30,
            monster.Position,
            Color.White,
            4,
            Game1.random.NextBool(),
            50f,
            duration / 200)
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<BurnAnimationUpdateTickedEvent>();
        EventManager.Enable<BurnAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, BurnAnimation> BurnAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0)
        {
            BurnAnimationByMonster.Remove(monster);
            return result;
        }

        this.Position = monster.GetOverheadOffset(time) - new Vector2(0, 8f);
        return result;
    }
}
