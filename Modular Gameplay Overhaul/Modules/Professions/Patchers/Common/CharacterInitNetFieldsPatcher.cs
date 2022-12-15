namespace DaLion.Overhaul.Modules.Professions.Patchers.Common;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CharacterInitNetFieldsPatcher"/> class.</summary>
    internal CharacterInitNetFieldsPatcher()
    {
        this.Target = this.RequireMethod<Character>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void CharacterInitNetFieldsPostfix(Character __instance)
    {
        if (__instance is Farmer { Name: { } } farmer)
        {
            __instance.NetFields.AddFields(farmer.Get_UltimateIndex());
            __instance.NetFields.AddFields(farmer.Get_IsHuntingTreasure());
        }
    }

    #endregion harmony patches
}
