namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Globalization;
using System.Text;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

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
    private static void IClickableMenuDrawHoverTextPrefix(
        ref string[]? buffIconsToDisplay,
        Item? hoveredItem)
    {
        if (hoveredItem is not SObject @object || !@object.isForage() ||
            !Game1.player.HasProfession(Profession.Ecologist, true) ||
            !State.PrestigedEcologistBuffsLookup.TryGetValue(@object.ItemId, out var buffIndex))
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

    #endregion harmony patches
}
