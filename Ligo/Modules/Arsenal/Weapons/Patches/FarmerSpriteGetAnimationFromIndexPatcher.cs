namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;
using StardewValley;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

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
        if (owner.CurrentTool is not MeleeWeapon weapon)
        {
            return true; // run original logic
        }

        var multiplier = 10f / (10f + weapon.speed.Value + weapon.Read<float>(DataFields.ResonantSpeed)) * (1f - owner.weaponSpeedModifier);
        var cooldown = 800 / (weapon.type.Value == MeleeWeapon.club ? 5 : 8);
        requester.loopThisAnimation = false;
        var outFrames = requester.currentAnimation;
        outFrames.Clear();
        switch (index)
        {
            case 248: // swing up
                outFrames.Add(new AnimationFrame(36, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(37, (int)(45 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(38, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(39, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(40, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(41, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true));
                break;
            case 240: // swing right
                outFrames.Add(new AnimationFrame(30, (int)(55 * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(31, (int)(45 * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(32, (int)(25 * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(33, (int)(25 * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(34, (int)(25 * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(35, (int)(cooldown * multiplier), secondaryArm: true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(35, 0, secondaryArm: true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true));
                break;
            case 232: // swing down
                outFrames.Add(new AnimationFrame(24, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(25, (int)(45 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(26, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(27, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(28, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(29, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true));
                break;
            case 256: // swing left
                outFrames.Add(new AnimationFrame(30, (int)(55 * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(31, (int)(45 * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(32, (int)(25 * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(33, (int)(25 * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(34, (int)(25 * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(35, (int)(cooldown * multiplier), secondaryArm: true, flip: true, Farmer.showSwordSwipe));
                outFrames.Add(new AnimationFrame(35, 0, secondaryArm: true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true));
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
