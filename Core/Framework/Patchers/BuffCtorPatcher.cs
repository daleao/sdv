namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.TokenizableStrings;
using BuffEnum = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages __instance patcher.</param>
    internal BuffCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<Buff>(10);
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool BuffCtorPrefix(
        Buff __instance,
        string? id,
        string? source = null,
        string? displaySource = null,
        int duration = -1,
        Texture2D? iconTexture = null,
        int iconSheetIndex = -1,
        BuffEffects? effects = null,
        bool? isDebuff = null,
        string? displayName = null,
        string? description = null)
    {
        if (!Config.ConsistentFarmerDebuffs || id is not ("12" or "14" or "19" or "27") ||
            !DataLoader.Buffs(Game1.content).TryGetValue(id, out var data))
        {
            return true; // run original logic
        }

        __instance.displayName = TokenParser.ParseText(data.DisplayName);
        __instance.description = TokenParser.ParseText(data.Description);
        __instance.glow = Utility.StringToColor(data.GlowColor) ?? __instance.glow;
        __instance.millisecondsDuration = data.MaxDuration > 0 && data.MaxDuration > data.Duration ? Game1.random.Next(data.Duration, data.MaxDuration + 1) : data.Duration;
        __instance.iconTexture = data.IconTexture == "TileSheets\\BuffsIcons" ? Game1.buffsIcons : Game1.content.Load<Texture2D>(data.IconTexture);
        __instance.iconSheetIndex = data.IconSpriteIndex;
        __instance.effects.Add(data.Effects);
        __instance.actionsOnApply = data.ActionsOnApply?.ToArray();
        var defaultIsDebuff = data.IsDebuff;
        __instance.customFields.TryAddMany(data.CustomFields);

        switch ((BuffEnum)int.Parse(id))
        {
            case BuffEnum.Burnt:
                var amount = (int)(Game1.player.maxHealth / 16f);
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.453") +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Damage() +
                                         Environment.NewLine + I18n.Ui_Buffs_Burnt_Dot(amount);
                __instance.effects.Clear();
                __instance.glow = Color.Yellow;
                __instance.millisecondsDuration = 15000;
                break;

            case BuffEnum.Jinxed:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.464") +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Defense() +
                                         Environment.NewLine + I18n.Ui_Buffs_Jinxed_Special();
                __instance.effects.Clear();
                __instance.glow = Color.HotPink;
                __instance.millisecondsDuration = 8000;
                break;

            case BuffEnum.Frozen:
                __instance.description = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.466") +
                                         Environment.NewLine + I18n.Ui_Buffs_Frozen_Stuck() + Environment.NewLine +
                                         I18n.Ui_Buffs_Frozen_Vulnerable();
                __instance.effects.Clear();
                __instance.effects.Speed.Value = int.MinValue;
                __instance.glow = Color.PowderBlue;
                __instance.millisecondsDuration = 5000;
                break;

            case BuffEnum.Weakness:
                __instance.displayName = I18n.Ui_Buffs_Confused_Name();
                __instance.description = I18n.Ui_Buffs_Confused_Desc();
                __instance.effects.Clear();
                __instance.glow = new Color(0, 150, 255);
                __instance.millisecondsDuration = 3000;
                break;

            default:
                return true; // run original logic
        }

        if (duration != -1)
        {
            __instance.millisecondsDuration = duration;
        }

        if (iconTexture != null)
        {
            __instance.iconTexture = iconTexture;
        }

        if (iconSheetIndex != -1)
        {
            __instance.iconSheetIndex = iconSheetIndex;
        }

        if (displayName != null)
        {
            __instance.displayName = displayName;
        }

        if (description != null)
        {
            __instance.description = description;
        }

        if (isDebuff.GetValueOrDefault(defaultIsDebuff) && Game1.player.isWearingRing("525") && __instance.millisecondsDuration != -2)
        {
            __instance.millisecondsDuration /= 2;
        }

        __instance.totalMillisecondsDuration = __instance.millisecondsDuration;
        if (effects != null)
        {
            __instance.effects.Add(effects);
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
