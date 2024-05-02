namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationInitNetFieldsPatcher"/> class.</summary>
    internal GameLocationInitNetFieldsPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void GameLocationInitNetFieldsPostfix(GameLocation __instance)
    {
        __instance.NetFields.AddFields(__instance.Get_Animals());
    }

    #endregion harmony patches
}
