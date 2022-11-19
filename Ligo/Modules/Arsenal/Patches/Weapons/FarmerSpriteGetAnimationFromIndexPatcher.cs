namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
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

        var owner = ModEntry.Reflector.GetUnboundFieldGetter<FarmerSprite, Farmer>(requester, "owner")
            .Invoke(requester);
        if (!owner.IsLocalPlayer || owner.CurrentTool is not MeleeWeapon weapon)
        {
            return true; // run original logic
        }

        var type = weapon.type.Value;
        switch (type)
        {
            case MeleeWeapon.club:
            {
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    owner.QueueForwardSwipe(weapon);
                }
                else
                {
                    owner.QueueOverheadSwipe(weapon);
                }

                break;
            }

            case MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword:
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    owner.QueueForwardSwipe(weapon);
                }
                else
                {
                    owner.QueueBackwardSwipe(weapon);
                }

                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
