namespace DaLion.Stardew.Arsenal;

#region using directives

using DaLion.Stardew.Arsenal.Framework;

#endregion using directives

internal sealed class ModState
{
    internal int EnergizeStacks { get; set; } = -1;

    internal ComboHitStep ComboHitStep { get; set; } = ComboHitStep.Idle;
}
