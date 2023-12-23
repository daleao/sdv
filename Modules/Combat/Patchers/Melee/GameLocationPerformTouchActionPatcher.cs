namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformTouchActionPatcher"/> class.</summary>
    internal GameLocationPerformTouchActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Add Galaxy/Infinity sword conversion option.</summary>
    [HarmonyPostfix]
    private static void GameLocationPerformTouchActionPostfix(GameLocation __instance, string fullActionString)
    {
        if (!CombatModule.Config.WeaponsSlingshots.EnableOverhaul || !CombatModule.Config.WeaponsSlingshots.EnableStabbingSwords ||
            fullActionString.Split()[0] != "legendarySword")
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.GalaxySword or WeaponIds.InfinityBlade } weapon ||
            CombatModule.State.UsedSandPillarsToday)
        {
            return;
        }

        var sword = weapon.DisplayName;
        var newType = weapon.type.Value switch
        {
            MeleeWeapon.stabbingSword => I18n.Weapons_Type_Defense(),
            MeleeWeapon.defenseSword => I18n.Weapons_Type_Stabbing(),
            _ => "???",
        };

        __instance.createQuestionDialogue(
            I18n.Pillars_ConvertSword(sword, newType),
            __instance.createYesNoResponses(),
            "PillarsConvert");
    }

    #endregion harmony patches
}
