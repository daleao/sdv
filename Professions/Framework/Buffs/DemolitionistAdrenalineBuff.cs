namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class DemolitionistAdrenalineBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.DemolitionistExcitedness";

    internal DemolitionistAdrenalineBuff()
        : base(
            id: ID,
            source: "Demolitionist",
            displaySource: _I18n.Get("demolitionist.title" + (Game1.player.IsMale ? ".male" : ".female")),
            iconTexture: null,
            duration: 555,
            effects: new BuffEffects { Speed = { State.DemolitionistAdrenaline }, },
            description: I18n.Demolitionist_Buff_Desc())
    {
    }
}
