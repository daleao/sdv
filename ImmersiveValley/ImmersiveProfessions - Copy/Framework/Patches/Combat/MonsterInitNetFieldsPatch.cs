namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterInitNetFieldsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterInitNetFieldsPatch()
    {
        Target = RequireMethod<Monster>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net debuffs.</summary>
    [HarmonyPostfix]
    private static void MonsterInitNetFieldsPostix(Monster __instance)
    {
        __instance.NetFields.AddFields(__instance.get_SlowIntensity());
        __instance.NetFields.AddFields(__instance.get_SlowTimer());
        __instance.NetFields.AddFields(__instance.get_FearIntensity());
        __instance.NetFields.AddFields(__instance.get_FearTimer());
    }

    #endregion harmony patches
}