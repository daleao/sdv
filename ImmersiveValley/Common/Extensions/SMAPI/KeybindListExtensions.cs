namespace DaLion.Common.Extensions.SMAPI;

#region using directives

using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="KeybindList"/> class.</summary>
public static class KeybindListExtensions
{
    /// <summary>Determines whether a <see cref="KeybindList"/> shares any <see cref="Keybind"/> with another <see cref="KeybindList"/>.</summary>
    /// <param name="b">A <see cref="KeybindList"/> to compare with.</param>
    public static bool HasCommonKeybind(this KeybindList a, KeybindList b) =>
        (from keybindA in a.Keybinds
         from keybindB in b.Keybinds
         let buttonsA = new HashSet<SButton>(keybindA.Buttons)
         let buttonsB = new HashSet<SButton>(keybindB.Buttons)
         where buttonsA.SetEquals(buttonsB)
         select buttonsA).Any();
}