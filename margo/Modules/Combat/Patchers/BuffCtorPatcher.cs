namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using BuffEnum = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffCtorPatcher"/> class.</summary>
    internal BuffCtorPatcher()
    {
        this.Target = this.RequireConstructor<Buff>(typeof(int));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool BuffCtorPrefix(Buff __instance, int which)
    {
        if (!CombatModule.Config.EnableStatusConditions)
        {
            return true; // run original logic
        }

        __instance.buffAttributes = new int[12];
        __instance.which = which;
        __instance.sheetIndex = which;
        switch ((BuffEnum)which)
        {
            case BuffEnum.Burnt:
                var amount = (int)(Game1.player.maxHealth / 16f);
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.453") +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Damage() +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Dot(amount);
                __instance.glow = Color.Yellow;
                __instance.millisecondsDuration = 15000;
                break;

            case BuffEnum.Jinxed:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.464") +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Defense() +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Special();
                __instance.buffAttributes[10] = -5;
                __instance.glow = Color.HotPink;
                __instance.millisecondsDuration = 8000;
                break;

            case BuffEnum.Frozen:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.466") +
                                         Environment.NewLine + I18n.Ui_Buffs_Frozen_Stuck() + Environment.NewLine +
                                         I18n.Ui_Buffs_Frozen_Vulnerable();
                __instance.glow = Color.PowderBlue;
                __instance.millisecondsDuration = 5000;
                break;

            case BuffEnum.Weakness:
                __instance.description = I18n.Ui_Buffs_Confused();
                __instance.glow = new Color(0, 150, 255);
                __instance.millisecondsDuration = 3000;
                CombatModule.State.MovementDirections.Shuffle(Game1.random);
                break;

            default:
                return true; // run original logic
        }

        if (Game1.player.isWearingRing(525))
        {
            __instance.millisecondsDuration /= 2;
        }

        __instance.totalMillisecondsDuration = __instance.millisecondsDuration;
        __instance.totalMillisecondsDuration = __instance.millisecondsDuration;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
