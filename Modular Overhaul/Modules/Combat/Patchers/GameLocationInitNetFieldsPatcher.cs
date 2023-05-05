namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
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

    [HarmonyPostfix]
    private static void GameLocationInitNetFieldsPostfix(GameLocation __instance)
    {
        __instance.characters.OnValueRemoved += NpcExtensions.OnRemoved;
    }

    #endregion harmony patches
}
