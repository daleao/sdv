namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Events;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a bleeding <see cref="Monster"/>.</summary>
public class BleedAnimation
{
    private readonly List<TemporaryAnimatedSprite> _droplets = new();
    private readonly Monster _attachedMonster;
    private float _timer;

    /// <summary>Initializes a new instance of the <see cref="BleedAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public BleedAnimation(Monster monster, int duration)
    {
        this._attachedMonster = monster;
        EventManager.Enable<BleedAnimationUpdateTickedEvent>();
        EventManager.Enable<BleedAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, BleedAnimation> BleedAnimationByMonster { get; } = new();

    /// <summary>Draws the animation to the specified <see cref="SpriteBatch"/>.</summary>
    /// <param name="b">The <see cref="SpriteBatch"/>.</param>
    public void Draw(SpriteBatch b)
    {
        this._droplets.ForEach(droplet => droplet.draw(b));
    }

    /// <summary>Updates the animation state.</summary>
    /// <param name="time">The current <see cref="GameTime"/>.</param>
    public void Update(GameTime time)
    {
        if (this._attachedMonster.Health <= 0)
        {
            this._droplets.Clear();
            BleedAnimationByMonster.Remove(this._attachedMonster);
            return;
        }

        for (var i = this._droplets.Count - 1; i >= 0; i--)
        {
            if (this._droplets[i].update(time))
            {
                this._droplets.RemoveAt(i);
            }
        }

        this._timer += time.ElapsedGameTime.Milliseconds;
        if (this._timer <= 2000f)
        {
            return;
        }

        this._timer = 0f;
        var offset = this._attachedMonster.GetOverheadOffset(time);
        for (var i = 0; i < 3; i++)
        {
            var flipped = this._attachedMonster.xVelocity < -1f || (!(this._attachedMonster.xVelocity > 1f) && Game1.random.NextBool());
            this._droplets.Add(new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors",
                new Rectangle(366, 412, 5, 6),
                offset + new Vector2(Game1.random.Next(32), 0f),
                flipped,
                0.027f,
                Color.Maroon)
            {
                motion = new Vector2(-this._attachedMonster.xVelocity / 2f, this._attachedMonster.yVelocity / 2f) +
                         new Vector2(
                             flipped ? 1.5f : -1.5f,
                             (this._attachedMonster.Sprite.SpriteHeight / -4f) + Game1.random.Next(-1, 2)),
                acceleration = new Vector2(0f, 0.5f),
                scale = 4f,
                delayBeforeAnimationStart = i * 150,
                attachedCharacter = this._attachedMonster,
                positionFollowsAttachedCharacter = true,
            });
        }
    }
}
