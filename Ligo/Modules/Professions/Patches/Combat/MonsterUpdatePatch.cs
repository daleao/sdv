namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.VirtualProperties;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdatePatch"/> class.</summary>
    internal MonsterUpdatePatch()
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.update), new[] { typeof(GameTime), typeof(GameLocation) });
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Patch to implement slow.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterUpdatePrefix(Monster __instance, GameTime time)
    {
        var slowTimer = __instance.Get_SlowTimer();
        if (slowTimer.Value <= 0)
        {
            return true; // run original logic
        }

        slowTimer.Value -= time.ElapsedGameTime.Milliseconds;
        var slowIntensity = __instance.Get_SlowIntensity();
        __instance.startGlowing(Color.LimeGreen, false, 0.05f);
        return time.TotalGameTime.Ticks % slowIntensity.Value == 0; // conditionally run original logic
    }

    #endregion harmony patches
}
