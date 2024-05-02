namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Core.Framework;

#endregion using directives

internal sealed class BruteRageBuff : StackableBuff
{
    internal const string ID = "DaLion.Professions.Buffs.BruteRage";
    internal const int SHEET_INDEX = 48;
    internal const int MAX_STACKS = 100;

    internal BruteRageBuff()
        : base(
            id: ID,
            getStacks: () => State.BruteRageCounter,
            maxStacks: MAX_STACKS,
            source: "Brute",
            displaySource: _I18n.Get("brute.title" + (Game1.player.IsMale ? ".male" : ".female")) + " " +
                           I18n.Brute_Buff_Name(),
            duration: 17,
            iconSheetIndex: SHEET_INDEX,
            getDescription: stacks =>
                I18n.Brute_Buff_Desc((stacks * 0.01f).ToString("P1"), (stacks * 0.005f).ToString("P1")))
    {
    }

    public override int MaxStacks => MAX_STACKS;
}
