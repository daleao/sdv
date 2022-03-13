namespace DaLion.Stardew.Tweaks.Framework.Patches;

#region using directives

using HarmonyLib;

#endregion using directives

/// <summary>Interface for Harmony patch classes.</summary>
internal interface IPatch
{
    /// <summary>Apply internally-defined Harmony patches.</summary>
    /// <param name="harmony">The Harmony instance for this mod.</param>
    public void Apply(Harmony harmony);
}