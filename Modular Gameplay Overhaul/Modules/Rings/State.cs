namespace DaLion.Overhaul.Modules.Rings;

/// <summary>The user-configurable settings for Rings.</summary>
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
            Game1.player.attack += value - this._warriorKillCount;
            this._warriorKillCount = value;
        }
    }

    internal int SavageExcitedness { get; set; }
}
