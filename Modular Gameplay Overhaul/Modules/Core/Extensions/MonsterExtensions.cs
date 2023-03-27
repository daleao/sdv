namespace DaLion.Overhaul.Modules.Core.Extensions;

using DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Overhaul.Modules.Core.Animations;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>Stuns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    internal static void Stun(this Monster monster, int duration)
    {
        monster.stunTime = duration;
        //monster.currentLocation.TemporarySprites.Add(new StunAnimation(monster, duration));
        StunAnimation.StunAnimationByMonster.AddOrUpdate(monster, new StunAnimation(monster, duration));
        EventManager.Enable<StunAnimationUpdateTickedEvent>();
        EventManager.Enable<StunAnimationRenderedWorldEvent>();
    }
}
