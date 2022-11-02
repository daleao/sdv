namespace DaLion.Redux.Framework.Arsenal.Extensions;

#region using directives

using DaLion.Redux.Framework.Arsenal.VirtualProperties;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>Decrements the knockback damage immunity cooldown.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void CountdownKnockbackCooldown(this Monster monster)
    {
        var current = monster.Get_KnockbackCooldown();
        if (current > 0)
        {
            monster.Set_KnockbackCooldown(current - 1);
        }
        else
        {
            ModEntry.State.Arsenal.KnockbackImmuneMonsters.Remove(monster);
        }
    }
}
