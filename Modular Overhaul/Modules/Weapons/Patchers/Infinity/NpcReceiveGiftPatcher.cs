namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

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
    private static void CommunityUpgradeAcceptPostfix(SObject o, Farmer giver)
    {
        giver.Increment(DataKeys.ProvenGenerosity, o.sellToStorePrice());
    }

    #endregion harmony patches
}
