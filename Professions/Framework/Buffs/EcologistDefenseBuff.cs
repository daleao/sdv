namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class EcologistDefenseBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.EcologistP.Defense";
    internal const int SHEET_INDEX = 10;

    internal EcologistDefenseBuff(float intensity)
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
            return new BuffEffects { Defense = { current.effects.Defense.Value + added } };
        }

        return new BuffEffects { Defense = { added } };
    }
}
