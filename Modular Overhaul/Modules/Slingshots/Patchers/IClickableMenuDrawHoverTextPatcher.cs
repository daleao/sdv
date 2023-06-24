namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Overhaul.Modules.Tools;
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

    /// <summary>Adds "Forged" text to Slingshots.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    new[] { new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)) },
                    () => helper.SetOperand(typeof(Tool)));
        }
        catch (Exception ex)
        {
            Log.E($"Failed generalizing enchantment tooltips to tools.\nHelper returned {ex}");
            return null;
        }

        // hack to fix price tag position in tooltip
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Item).RequireMethod(nameof(Item.attachmentSlots))),
                    },
                    ILHelper.SearchOption.Last)
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 12),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding buffer space between ammo slots and money required.\nHelper returned {ex}");
            return null;
        }

        if (WeaponsModule.ShouldEnable)
        {
            return helper.Flush();
        }

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
            Log.E($"Failed modifying hovered tool title color.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static Color GetTitleColorFor(Item? item)
    {
        if (item is not Slingshot slingshot || !SlingshotsModule.Config.ColorCodedForYourConvenience)
        {
            return Game1.textColor;
        }

        var bowData = ArcheryIntegration.Instance!.ModApi!.GetWeaponData(Manifest, slingshot);
        if (bowData is null)
        {
            return slingshot.InitialParentTileIndex switch
            {
                ItemIDs.GalaxySlingshot => Color.DarkViolet,
                ItemIDs.InfinitySlingshot => Color.DeepPink,
                _ => Game1.textColor,
            };
        }

        if (slingshot.Name.Contains("Copper"))
        {
            return UpgradeLevel.Copper.GetTextColor();
        }

        if (slingshot.Name.Contains("Steel"))
        {
            return UpgradeLevel.Steel.GetTextColor();
        }

        if (slingshot.Name.Contains("Gold"))
        {
            return UpgradeLevel.Gold.GetTextColor();
        }

        if (slingshot.Name.Contains("Iridium"))
        {
            return UpgradeLevel.Iridium.GetTextColor();
        }

        if (slingshot.Name.Contains("Yoba"))
        {
            return Color.Gold;
        }

        if (slingshot.Name.Contains("Dwarven"))
        {
            return Color.MonoGameOrange;
        }

        return Game1.textColor;
    }

    #endregion injected subroutines
}
