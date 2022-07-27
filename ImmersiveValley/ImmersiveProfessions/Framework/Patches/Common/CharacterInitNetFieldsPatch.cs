namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CharacterInitNetFieldsPatch()
    {
        Target = RequireMethod<Character>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void CharacterInitNetFieldsPostfix(Character __instance)
    {
        if (__instance is not Farmer farmer) return;

        __instance.NetFields.AddFields(farmer.get_UltimateIndex());
        __instance.NetFields.AddFields(farmer.get_IsUltimateActive());
        __instance.NetFields.AddFields(farmer.get_IsFake());
    }

    #endregion harmony patches
}