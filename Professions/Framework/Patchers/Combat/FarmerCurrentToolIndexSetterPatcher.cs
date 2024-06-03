namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerCurrentToolIndexSetterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Set Rascal ammo slots.</summary>
    [HarmonyPrefix]
    private static void FarmerCurrentToolIndexPrefix(Farmer __instance, int value)
    {
        if (value < 0 || value >= __instance.Items.Count || __instance.Items[value] is not Slingshot slingshot)
        {
            return;
        }

        if (!__instance.HasProfession(Profession.Rascal) ||
            (slingshot.AttachmentSlotsCount == 2 && slingshot.attachments.Length == 2))
        {
            return;
        }

        var replacement = ItemRegistry.Create<Slingshot>(slingshot.QualifiedItemId);
        replacement.AttachmentSlotsCount = 2;
        if (slingshot.attachments[0] is { } ammo)
        {
            replacement.attachments[0] = (SObject)ammo.getOne();
            replacement.attachments[0].Stack = ammo.Stack;
        }

        for (var j = slingshot.enchantments.Count - 1; j >= 0; j--)
        {
            var enchantment = slingshot.enchantments[j];
            replacement.AddEnchantment(enchantment);
            slingshot.RemoveEnchantment(enchantment);
        }

        __instance.Items[value] = replacement;
    }

    #endregion harmony patches
}
