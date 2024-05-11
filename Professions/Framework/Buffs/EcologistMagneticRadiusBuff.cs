namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class EcologistMagneticRadiusBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.EcologistP.MagneticRadius";

    internal EcologistMagneticRadiusBuff(float intensity = 32f)
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
        if (Game1.player.buffs.AppliedBuffs.TryGetValue(ID, out var current))
        {
            return new BuffEffects { MagneticRadius = { current.effects.MagneticRadius.Value + added }, };
        }

        return new BuffEffects { MagneticRadius = { added } };
    }
}
