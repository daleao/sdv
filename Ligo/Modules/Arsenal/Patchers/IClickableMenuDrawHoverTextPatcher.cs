namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IClickableMenuDrawHoverTextPatcher"/> class.</summary>
    internal IClickableMenuDrawHoverTextPatcher()
    {
        this.Target = this.RequireMethod<IClickableMenu>(
            nameof(IClickableMenu.drawHoverText),
            new[]
            {
                typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
                typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int),
                typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>),
            });
    }

    #region harmony patches

    /// <summary>Set hover text color for legendary weapons.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
        // To: b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4), GetTitleColorFor(hoveredItem);
        try
        {
            helper
                .Match(// find second occurrence of `if (bold_title_subtext != null)`
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0), // local 0 = string bold_title_subtext
                        new CodeInstruction(OpCodes.Brfalse_S),
                    },
                    ILHelper.SearchOption.Last)
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.textColor))) },
                    ILHelper.SearchOption.Previous)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(IClickableMenuDrawHoverTextPatcher).RequireMethod(nameof(GetTitleColorFor))))
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)9) }); // arg 10 = Item item
        }
        catch (Exception ex)
        {
            Log.E($"Failed modifying hovered weapon title color.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static Color GetTitleColorFor(Item? item)
    {
        if (item is null || !ArsenalModule.Config.ColorCodedForYourConvenience)
        {
            return Game1.textColor;
        }

        switch (item)
        {
            case MeleeWeapon weapon:
                var tier = WeaponTier.GetFor(weapon);
                if (tier < WeaponTier.Legendary)
                {
                    return tier.Color;
                }

                if (weapon.isGalaxyWeapon())
                {
                    return Color.DarkViolet;
                }

                if (weapon.IsInfinityWeapon())
                {
                    return Color.DeepPink;
                }

                switch (weapon.InitialParentTileIndex)
                {
                    case Constants.DarkSwordIndex:
                        return Color.DarkSlateGray;
                    case Constants.HolyBladeIndex:
                        return Color.Gold;
                }

                goto default;
            case Slingshot slingshot:
                switch (slingshot.InitialParentTileIndex)
                {
                    case Constants.GalaxySlingshotIndex:
                        return Color.Purple;
                    case Constants.InfinitySlingshotIndex:
                        return Color.DeepPink;
                    case Constants.MasterSlingshotIndex:
                        return Color.Green;
                }

                goto default;
            default:
                return Game1.textColor;
        }
    }

    #endregion injected subroutines
}
