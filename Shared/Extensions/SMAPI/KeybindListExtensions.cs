 namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.Linq;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Extensions for the <see cref="KeybindList"/> class.</summary>
public static class KeybindListExtensions
{
    /// <summary>Determines whether the <paramref name="keybinds"/> has the specified <see cref="SButton"/> as a <see cref="Keybind"/>.</summary>
    /// <param name="keybinds">The <see cref="KeybindList"/>.</param>
    /// <param name="button">Some <see cref="SButton"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="button"/> is a valid <see cref="Keybind"/> in <paramref name="keybinds"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasButton(this KeybindList keybinds, SButton button)
    {
        return keybinds.Keybinds.Any(k =>
            k.Buttons.Length == 1 &&
            k.Buttons[0] == button);
    }

    /// <summary>Determines whether the <paramref name="keybinds"/> has the specified <see cref="Keybind"/>.</summary>
    /// <param name="keybinds">The <see cref="KeybindList"/>.</param>
    /// <param name="keybind">Some <see cref="Keybind"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="keybind"/> is a valid <see cref="Keybind"/> in <paramref name="keybinds"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasKeybind(this KeybindList keybinds, Keybind keybind)
    {
        return keybinds.Keybinds.Any(k =>
            k.Buttons.Length == keybind.Buttons.Length &&
            k.Buttons.All(keybind.Buttons.Contains));
    }

    /// <summary>Determines whether the <paramref name="keybinds"/>/> shares any <see cref="Keybind"/>s with <paramref name="other"/>.</summary>
    /// <param name="keybinds">The <see cref="KeybindList"/>.</param>
    /// <param name="other">Some other <see cref="KeybindList"/> to compare with.</param>
    /// <returns><see langword="true"/> if <paramref name="keybinds"/> and <paramref name="other"/> share at least one <see cref="Keybind"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasCommonKeybind(this KeybindList keybinds, KeybindList other)
    {
        return keybinds.Keybinds.Any(a =>
            other.Keybinds.Any(b =>
                a.Buttons.Length == b.Buttons.Length &&
                a.Buttons.All(b.Buttons.Contains)));
    }
}
