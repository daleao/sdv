namespace DaLion.Stardew.Arsenal.Framework.Patches.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;

#endregion using directives

[UsedImplicitly]
internal class BaseEnchantmentGetAvailableEnchantmentsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    BaseEnchantmentGetAvailableEnchantmentsPatch()
    {
        Original = RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Allows applying magic/sunburst enchant.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BaseEnchantmentGetAvailableEnchantmentsTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        l.InsertRange(l.Count - 2, new List<CodeInstruction>
        {
            new(OpCodes.Ldsfld, typeof(BaseEnchantment).Field("_enchantments")),
            new(OpCodes.Newobj, typeof(MagicEnchantment).Constructor()),
            new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).MethodNamed(nameof(List<BaseEnchantment>.Add)))
        });

        return l.AsEnumerable();
    }

    #endregion harmony patches
}