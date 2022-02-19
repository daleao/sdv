using DaLion.Stardew.Professions.Framework.SuperMode;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeBehaviorAtGameTickPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeBehaviorAtGameTickPatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to countdown jump timers + make Slimes friendly towards Pipers.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeBehaviorAtGameTickPostfix(GreenSlime __instance, ref int ___readyToJump)
    {
        var timeLeft = __instance.ReadDataAs<int>("Jumping");
        if (timeLeft > 0)
        {
            timeLeft -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            __instance.WriteData("Jumping", timeLeft <= 0 ? null : timeLeft.ToString());
        }

        if (!__instance.Player.HasProfession(Profession.Piper)) return;
        
        ___readyToJump = -1;
        if (ModEntry.State.Value.SuperMode is PiperEubstance eubstance)
            __instance.addedSpeed = eubstance.GetBonusSlimeMovementSpeed() + (__instance.isGlowing ? 2 : 0);
    }

    #endregion harmony patches
}