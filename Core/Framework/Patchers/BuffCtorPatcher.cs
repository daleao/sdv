namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages __instance patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BuffCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<Buff>(10);
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BuffCtorPostfix(Buff __instance, string? id, bool? isDebuff = null)
    {
        if (!isDebuff.GetValueOrDefault() || !Config.ConsistentFarmerDebuffs)
        {
            return; // run original logic
        }

        switch (id)
        {
            case BuffIDs.Burnt:
                var amount = (int)(Game1.player.maxHealth / 16f);
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.453") +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Damage() +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Dot(amount);
                __instance.effects.Clear();
                __instance.glow = Color.Yellow;
                __instance.millisecondsDuration = 15000;
                break;

            case BuffIDs.Jinxed:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.464") +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Defense() +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Special();
                __instance.effects.Clear();
                var defense = Game1.player.buffs.FloatingDefense();
                __instance.effects.Defense.Value = defense / 2;
                __instance.glow = Color.HotPink;
                __instance.millisecondsDuration = 8000;
                break;

            case BuffIDs.Frozen:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.466") +
                                         Environment.NewLine + I18n.Ui_Buffs_Frozen_Stuck() + Environment.NewLine +
                                         I18n.Ui_Buffs_Frozen_Vulnerable();
                __instance.effects.Clear();
                __instance.effects.Speed.Value = int.MinValue;
                __instance.glow = Color.PowderBlue;
                __instance.millisecondsDuration = 5000;
                break;

            case BuffIDs.Weakness:
                __instance.displayName = I18n.Ui_Buffs_Confused_Name();
                __instance.description = I18n.Ui_Buffs_Confused_Desc();
                __instance.effects.Clear();
                __instance.glow = new Color(0, 150, 255);
                __instance.millisecondsDuration = 3000;
                break;

            default:
                return;
        }

        if (Game1.player.isWearingRing("525"))
        {
            __instance.millisecondsDuration /= 2;
        }

        __instance.totalMillisecondsDuration = __instance.millisecondsDuration;
    }

    #endregion harmony patches
}
