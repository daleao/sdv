namespace DaLion.Core.Framework;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buffs;

#endregion using directives

/// <summary>A <see cref="Buff"/> that can be stacked and displays a corresponding counter.</summary>
public abstract class StackableBuff : Buff
{
    private readonly Func<int> _getStacks;
    private readonly Func<int, string>? _getDescription;

    /// <inheritdoc cref="Buff"/>
    public StackableBuff(
        string id,
        Func<int> getStacks,
        int maxStacks,
        string? source = null,
        string? displaySource = null,
        int duration = -1,
        Texture2D? iconTexture = null,
        int iconSheetIndex = -1,
        BuffEffects? effects = null,
        bool isDebuff = false,
        string? displayName = null,
        Func<int, string>? getDescription = null)
        : base(
            id,
            source,
            displaySource,
            duration,
            iconTexture,
            iconSheetIndex,
            effects,
            isDebuff,
            displayName)
    {
        this._getStacks = getStacks;
        this._getDescription = getDescription;
        this.description = this._getDescription?.Invoke(this._getStacks());
    }

    /// <summary>Gets the current number of stacks.</summary>
    public int Stacks => this._getStacks();

    /// <summary>Gets the maximum number of stacks for this buff.</summary>
    public abstract int MaxStacks { get; }
}
