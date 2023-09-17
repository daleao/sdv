namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcReceiveGiftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcReceiveGiftPatcher"/> class.</summary>
    internal NpcReceiveGiftPatcher()
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.receiveGift));
    }

    #region harmony patches

    /// <summary>Complete Generosity quest.</summary>
    [HarmonyPostfix]
    private static void NpcReceiveGiftPostfix(SObject o, Farmer giver)
    {
        giver.Increment(Virtue.Generosity.Name, o.sellToStorePrice());
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Generosity);
    }

    #endregion harmony patches
}
