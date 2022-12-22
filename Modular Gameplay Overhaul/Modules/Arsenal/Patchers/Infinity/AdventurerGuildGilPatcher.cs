namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class AdventurerGuildGilPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AdventurerGuildGilPatcher"/> class.</summary>
    internal AdventurerGuildGilPatcher()
    {
        this.Target = this.RequireMethod<AdventureGuild>("gil");
    }

    #region harmony patches

    /// <summary>Record Gil flag.</summary>
    [HarmonyPostfix]
    private static void AdventurerGuildGilPostfix(bool ___talkedToGil)
    {
        var player = Game1.player;
        if (player.NumMonsterSlayerQuestsCompleted() >= 5)
        {
            player.Write(DataFields.ProvenValor, true.ToString());
            Virtue.Valor.CheckForCompletion(player);
        }

        if (!player.hasQuest(Constants.VirtuesIntroQuestId) || !___talkedToGil)
        {
            return;
        }

        if (player.Read<bool>(DataFields.TalkedToYoba))
        {
            player.completeQuest(Constants.VirtuesIntroQuestId);
            player.Write(DataFields.TalkedToYoba, null);
        }
        else
        {
            player.Write(DataFields.TalkedToGil, true.ToString());
        }
    }

    #endregion harmony patches
}
