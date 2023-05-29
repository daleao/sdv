namespace DaLion.Overhaul.Modules.Rings;

/// <summary>The runtime state variable for RNGS.</summary>
internal sealed class State
{
    private int _warriorKillCount;

    internal int WarriorKillCount
    {
        get
        {
            return this._warriorKillCount;
        }

        set
        {
            this._warriorKillCount = Math.Min(value, 20);
        }
    }

    internal int SavageExcitedness { get; set; }

    internal int YobaShieldHealth { get; set; } = -1;

    internal bool CanReceiveYobaShield { get; set; } = true;
}
