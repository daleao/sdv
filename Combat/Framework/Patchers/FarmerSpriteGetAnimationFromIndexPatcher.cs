﻿namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerSpriteGetAnimationFromIndexPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerSpriteGetAnimationFromIndexPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerSpriteGetAnimationFromIndexPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FarmerSprite>(nameof(FarmerSprite.getAnimationFromIndex));
    }

    #region harmony patches

    /// <summary>Do weapon combo.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerSpriteGetAnimationFromIndexPrefix(int index, FarmerSprite requester)
    {
        if (!Config.EnableComboHits || index is not (248 or 240 or 232 or 256))
        {
            return true; // run original logic
        }

        try
        {
            var owner = Reflector.GetUnboundFieldGetter<FarmerSprite, Farmer>("owner")
                .Invoke(requester);
            if (!owner.IsLocalPlayer || owner.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
            {
                return true; // run original logic
            }

            var hitStep = State.QueuedHitStep;
            if (weapon.IsClub() && hitStep == weapon.GetFinalHitStep() - 1)
            {
                owner.QueueSmash(weapon);
            }
            else if ((int)hitStep % 2 == 0)
            {
                owner.QueueForwardSwipe(weapon);
            }
            else
            {
                owner.QueueReverseSwipe(weapon);
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
