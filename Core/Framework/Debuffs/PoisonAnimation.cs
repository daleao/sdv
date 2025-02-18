﻿namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Core.Framework.Events;
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
            $"{Manifest.UniqueID}_PoisonAnimation",
            new Rectangle(0, 0, 64, 128),
            50f,
            6,
            duration / 300,
            monster.Position,
            false,
            Game1.random.NextBool())
    {
        this.Position = new Vector2(0f, -64f);
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<PoisonAnimationUpdateTickedEvent>();
        EventManager.Enable<PoisonAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, PoisonAnimation> PoisonAnimationByMonster { get; } = [];

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0 || !monster.IsPoisoned() || !ReferenceEquals(monster.currentLocation, Game1.currentLocation))
        {
            PoisonAnimationByMonster.Remove(monster);
            return result;
        }

        this.Position = new Vector2(0f, -64f);
        return result;
    }
}
