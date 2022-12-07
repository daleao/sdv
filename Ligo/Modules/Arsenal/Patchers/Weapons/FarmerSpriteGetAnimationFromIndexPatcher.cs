namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerSpriteGetAnimationFromIndexPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerSpriteGetAnimationFromIndexPatcher"/> class.</summary>
    internal FarmerSpriteGetAnimationFromIndexPatcher()
    {
        this.Target = this.RequireMethod<FarmerSprite>(nameof(FarmerSprite.getAnimationFromIndex));
    }

    #region harmony patches

    /// <summary>Overhaul weapon swing speed to scale better with bonuses.</summary>
    [HarmonyPrefix]
    private static bool FarmerSpriteGetAnimationFromIndexPrefix(int index, FarmerSprite requester)
    {
        if (index is not (248 or 240 or 232 or 256))
        {
            return true; // run original logic
        }

        try
        {
            var owner = ModEntry.Reflector.GetUnboundFieldGetter<FarmerSprite, Farmer>(requester, "owner")
                .Invoke(requester);
            if (!owner.IsLocalPlayer || owner.CurrentTool is not MeleeWeapon weapon)
            {
                return true; // run original logic
            }

            var type = weapon.type.Value;
            var hitStep = owner.Get_CurrentHitStep();
            if (type == MeleeWeapon.club && hitStep == weapon.GetFinalHitStep() - 1)
            {
                owner.QueueOverheadSwipe(weapon);
            }
            else if ((int)hitStep % 2 == 0 || weapon.isScythe())
            {
                owner.QueueForwardSwipe(weapon);
            }
            else
            {
                owner.QueueBackwardSwipe(weapon);
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
