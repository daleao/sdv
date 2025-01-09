namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Enchantments.Framework.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolRemoveEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolRemoveEnchantmentPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ToolRemoveEnchantmentPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.RemoveEnchantment));
    }

    #region harmony patches

    /// <summary>Disposes certain enchantments.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ToolRemoveEnchantmentPostifx(BaseEnchantment enchantment)
    {
        switch (enchantment)
        {
            case EnergizedMeleeEnchantment:
                EventManager.Disable<EnergizedUpdateTickedEvent>();
                break;
            case ExplosiveEnchantment:
                EventManager.Disable<ExplosiveUpdateTickedEvent>();
                break;
        }
    }

    #endregion harmony patches
}
