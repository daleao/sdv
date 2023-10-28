namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeBehaviorAtGameTickPatcher"/> class.</summary>
    internal GreenSlimeBehaviorAtGameTickPatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to countdown jump timers.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeBehaviorAtGameTickPostfix(GreenSlime __instance, ref int ___readyToJump)
    {
        var timeLeft = __instance.Get_JumpTimer();
        if (timeLeft <= 0)
        {
            return;
        }

        timeLeft -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        __instance.Set_JumpTimer(timeLeft);

        //if (!__instance.Player.HasProfession(Profession.Piper)) return;
        //  ___readyToJump = -1;
    }

    #endregion harmony patches
}
