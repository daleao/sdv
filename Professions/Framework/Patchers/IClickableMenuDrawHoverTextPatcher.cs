namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IClickableMenuDrawHoverTextPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal IClickableMenuDrawHoverTextPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<IClickableMenu>(nameof(IClickableMenu.drawHoverText), [
            typeof(SpriteBatch),
            typeof(StringBuilder),
            typeof(SpriteFont),
            typeof(int),
            typeof(int),
            typeof(int),
            typeof(string),
            typeof(int),
            typeof(string[]),
            typeof(Item),
            typeof(int),
            typeof(string),
            typeof(int),
            typeof(int),
            typeof(int),
            typeof(float),
            typeof(CraftingRecipe),
            typeof(IList<Item>),
            typeof(Texture2D),
            typeof(Rectangle?),
            typeof(Color?),
            typeof(Color?),
            typeof(float),
            typeof(int),
            typeof(int),
        ]);
    }

    #region harmony patches

    /// <summary>Draw Prestiged Ecologist buff tooltips.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void IClickableMenuDrawHoverTextPrefix(
        ref string[]? buffIconsToDisplay,
        Item? hoveredItem)
    {
        if (hoveredItem is not SObject @object || !@object.isForage() ||
            !Game1.player.HasProfession(Profession.Ecologist, true) ||
            !State.EcologistBuffsLookup.TryGetValue(@object.ItemId, out var buffIndex))
        {
            return;
        }

        buffIconsToDisplay ??= Enumerable.Repeat(string.Empty, 13).ToArray();
        switch (buffIndex)
        {
            // farming
            case 0:
                buffIconsToDisplay[0] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // fishing
            case 1:
                buffIconsToDisplay[1] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // mining
            case 2:
                buffIconsToDisplay[2] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // luck
            case 3:
                buffIconsToDisplay[4] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // foraging
            case 4:
                buffIconsToDisplay[5] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // max stamina
            case 5:
                buffIconsToDisplay[7] = 10f.ToString(CultureInfo.CurrentCulture);
                break;

            // magnetic radius
            case 6:
                buffIconsToDisplay[8] = 32f.ToString(CultureInfo.CurrentCulture);
                break;

            // speed
            case 7:
                buffIconsToDisplay[9] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // defense
            case 8:
                buffIconsToDisplay[10] = 0.5f.ToString(CultureInfo.CurrentCulture);
                break;

            // attack
            case 9:
                buffIconsToDisplay[11] = 5f.ToString(CultureInfo.CurrentCulture);
                break;

            default:
                return;
        }
    }

    /// <summary>Patch to flush Rascal slingshot tooltip.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var notSlingshot = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Item).RequireMethod(nameof(Item.attachmentSlots))),
                ])
                .Move(-2)
                .GetOperand(out var notFishingRod)
                .PatternMatch([ new CodeInstruction(OpCodes.Br_S)])
                .GetOperand(out var resumeExecution)
                .LabelMatch((Label)notFishingRod)
                .StripLabels(out _)
                .AddLabels(notSlingshot)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                        new CodeInstruction(OpCodes.Brfalse_S, notSlingshot),
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 68),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stloc_2),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    ],
                    [(Label)notFishingRod]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed flushing Rascal slingshot tooltip height.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
