namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BaseEnchantmentGetAvailableEnchantmentsPatch()
    {
        Target = RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Allow applying magic/sunburst enchant.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BaseEnchantmentGetAvailableEnchantmentsTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        l.InsertRange(l.Count - 2, new List<CodeInstruction>
        {
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
            new(OpCodes.Newobj, typeof(MagicEnchantment).RequireConstructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add)))
        });

        return l.AsEnumerable();
    }

    #endregion harmony patches
}