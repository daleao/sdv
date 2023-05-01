namespace DaLion.Overhaul.Modules.Core.UI;

/// <summary>A <see cref="Buff"/> that can be stacked and displays a corresponding counter.</summary>
public class StackableBuff : Buff
{
    private Func<int> _getStacks;

    /// <inheritdoc cref="Buff"/>
    public StackableBuff(
        string description,
        int millisecondsDuration,
        string source,
        int index,
        Func<int> getStacks,
        int maxStacks = -1)
        : base(description, millisecondsDuration, source, index)
    {
        this._getStacks = getStacks;
        this.MaxStacks = maxStacks;
    }

    /// <inheritdoc cref="Buff"/>
    public StackableBuff(
        int farming,
        int fishing,
        int mining,
        int digging,
        int luck,
        int foraging,
        int crafting,
        int maxStamina,
        int magneticRadius,
        int speed,
        int defense,
        int attack,
        int minutesDuration,
        string source,
        string displaySource,
        Func<int> getStacks,
        int maxStacks = -1)
        : base(
            farming,
            fishing,
            mining,
            digging,
            luck,
            foraging,
            crafting,
            maxStamina,
            magneticRadius,
            speed,
            defense,
            attack,
            minutesDuration,
            source,
            displaySource)
    {
        this._getStacks = getStacks;
        this.MaxStacks = maxStacks;
    }

    /// <summary>Gets the current number of stacks.</summary>
    public int Stacks => this._getStacks();

    /// <summary>Gets the maximum number of stacks for this buff.</summary>
    public int MaxStacks { get; }
}
