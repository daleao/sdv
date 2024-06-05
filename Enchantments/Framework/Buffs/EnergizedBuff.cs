namespace DaLion.Enchantments.Framework.Buffs;

#region using directives

using DaLion.Core.Framework;
using DaLion.Enchantments.Framework.Enchantments;

#endregion using directives

internal sealed class EnergizedBuff : StackableBuff
{
    internal const string ID = "DaLion.Enchantments.Buffs.Energized";
    private const int SHEET_INDEX = 58;

    internal EnergizedBuff(Func<int> getStacks)
        : base(
            id: ID,
            getStacks: getStacks,
            maxStacks: EnergizedMeleeEnchantment.MaxEnergy,
            source: "Energized",
            displaySource: I18n.Enchantments_Energized_Name(),
            duration: 17,
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            getDescription: stacks => I18n.Enchantments_Energized_Desc(stacks))
    {
    }
}
