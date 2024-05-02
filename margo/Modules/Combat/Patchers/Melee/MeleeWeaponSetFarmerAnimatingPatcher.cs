namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSetFarmerAnimatingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSetFarmerAnimatingPatcher"/> class.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Movement speed does not affect swing speed + remove weapon enchantment OnSwing effect (handled in custom logic).</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: swipeSpeed -= who.addedSpeed * 40;
        try
        {
            var skipMovementSpeed = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.addedSpeed))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.WeaponsSlingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(WeaponsSlingshotsConfig).RequirePropertyGetter(nameof(WeaponsSlingshotsConfig.EnableOverhaul))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipMovementSpeed),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Conv_R4) })
                .AddLabels(skipMovementSpeed);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing move speed's effect to swing speed.\nHelper returned {ex}");
            return null;
        }

        // From: if (who.IsLocalPlayer)
        // To: if (who.IsLocalPlayer && (this.type.Value == MeleeWeapon.dagger || !CombatModule.Config.EnableComboHits))
        // Before: foreach (BaseEnchantment enchantment in enchantments) if (enchantment is BaseWeaponEnchantment) (enchantment as BaseWeaponEnchantment).OnSwing(this, who);
        try
        {
            var doCheckEnchantments = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    },
                    ILHelper.SearchOption.First)
                .Move(2)
                .GetOperand(out var skipCheckEnchantments)
                .Move()
                .AddLabels(doCheckEnchantments)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // 1 = MeleeWeapon.dagger
                        new CodeInstruction(OpCodes.Beq_S, doCheckEnchantments),
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.WeaponsSlingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(WeaponsSlingshotsConfig).RequirePropertyGetter(nameof(WeaponsSlingshotsConfig.EnableComboHits))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipCheckEnchantments),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing enchantment on swing effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
