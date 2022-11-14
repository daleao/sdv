namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetAvailableEnchantmentsPatch"/> class.</summary>
    internal BaseEnchantmentGetAvailableEnchantmentsPatch()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Allow applying new enchants.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BaseEnchantmentGetAvailableEnchantmentsTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        l.InsertRange(l.Count - 2, new List<CodeInstruction>
        {
            // add engorging enchant
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(EngorgingEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
            // add gatling enchant
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(GatlingEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
            // add preserving enchant
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(Enchantments.PreservingEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
            // add quincy enchant
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(QuincyEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
            // add spreading enchant
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(SpreadingEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
        });

        return l.AsEnumerable();
    }

    #endregion harmony patches
}
