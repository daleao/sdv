namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class GreenSlimeBehaviorAtGameTickPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeBehaviorAtGameTickPatch"/> class.</summary>
    internal GreenSlimeBehaviorAtGameTickPatch()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to countdown jump timers.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeBehaviorAtGameTickPostfix(GreenSlime __instance, ref int ___readyToJump)
    {
        var timeLeft = __instance.Read<int>(DataFields.Jumping);
        if (timeLeft <= 0)
        {
            return;
        }

        timeLeft -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        __instance.Write(DataFields.Jumping, timeLeft <= 0 ? null : timeLeft.ToString());

        //if (!__instance.Player.HasProfession(Profession.Piper)) return;
        
        //___readyToJump = -1;
    }

    #endregion harmony patches
}
