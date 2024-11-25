namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class EcologistMaxStaminaBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.EcologistP.MaxStamina";

    internal EcologistMaxStaminaBuff(float intensity = 10f)
        : base(
            id: ID,
            source: "Ecologist",
            displaySource: _I18n.Get("ecologist.title.prestiged" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: 60000,
            effects: GetBuffEffects(intensity))
    {
    }

    private static BuffEffects GetBuffEffects(float added)
    {
        return Game1.player.buffs.AppliedBuffs.TryGetValue(ID, out var current)
            ? new BuffEffects { MaxStamina = { current.effects.MaxStamina.Value + added }, }
            : new BuffEffects { MaxStamina = { added } };
    }
}
