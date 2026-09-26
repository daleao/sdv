namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotPerformObjectDropInActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CrabPotPerformObjectDropInActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Fixes an issue when collecting trash while holding bait as Conservationist.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CrabPotPerformObjectDropInActionPrefix(CrabPot __instance, ref bool __result, Item dropInItem, bool probe, Farmer who)
    {
        if (__instance.heldObject.Value is null)
        {
            if (__instance.Location is not Caldera)
            {
                return true; // run original logic;
            }

            if (__instance.bait.Value is null && who is not null && who.HasProfession(Profession.Trapper, true))
            {
                if (dropInItem is SObject { QualifiedItemId: QIDs.MagicBait } magicBait)
                {
                    if (!probe)
                    {
                        __instance.owner.Value = who.UniqueMultiplayerID;
                        __instance.bait.Value = magicBait.getOne() as SObject;
                        __instance.Location.playSound("Ship");
                        __instance.lidFlapping = true;
                        __instance.lidFlapTimer = 60f;
                    }

                    __result = true;
                    return false; // don't run original logic
                }
            }
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
