namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class EcologistMiningBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.EcologistP.Mining";
    internal const int SHEET_INDEX = 2;

    internal EcologistMiningBuff(float intensity)
        : base(
            id: ID,
            source: "Ecologist",
            displaySource: _I18n.Get("ecologist.title.prestiged" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: 60000,
            iconSheetIndex: SHEET_INDEX,
            effects: GetBuffEffects(intensity))
    {
    }

    private static BuffEffects GetBuffEffects(float added)
    {
        if (Game1.player.buffs.AppliedBuffs.TryGetValue(ID, out var current))
        {
            return new BuffEffects { MiningLevel = { current.effects.MiningLevel.Value + added } };
        }

        return new BuffEffects { MiningLevel = { added } };
    }
}
