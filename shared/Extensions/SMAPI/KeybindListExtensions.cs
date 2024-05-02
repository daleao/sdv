 namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Extensions for the <see cref="KeybindList"/> class.</summary>
public static class KeybindListExtensions
{
    /// <summary>Determines whether the <paramref name="keybindList"/>/> shares any <see cref="Keybind"/>s with <paramref name="other"/>.</summary>
    /// <param name="keybindList">The <see cref="KeybindList"/>.</param>
    /// <param name="other">Some other <see cref="KeybindList"/> to compare with.</param>
    /// <returns><see langword="true"/> if <paramref name="keybindList"/> and <paramref name="other"/> share at least one <see cref="Keybind"/>.</returns>
    public static bool HasCommonKeybind(this KeybindList keybindList, KeybindList other)
    {
        return (from keybindA in keybindList.Keybinds
            from keybindB in other.Keybinds
            let buttonsA = new HashSet<SButton>(keybindA.Buttons)
            let buttonsB = new HashSet<SButton>(keybindB.Buttons)
            where buttonsA.SetEquals(buttonsB)
            select buttonsA).Any();
    }
}
