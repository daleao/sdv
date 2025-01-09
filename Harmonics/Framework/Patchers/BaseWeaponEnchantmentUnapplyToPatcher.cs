namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseWeaponEnchantmentUnapplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BaseWeaponEnchantmentUnapplyToPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.UnapplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }


    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return new[]
        {
            typeof(AmethystEnchantment), typeof(AquamarineEnchantment), typeof(EmeraldEnchantment),
            typeof(JadeEnchantment), typeof(RubyEnchantment), typeof(TopazEnchantment),
        }.Select(t => t.RequireMethod("_UnapplyTo"));
    }

    #region harmony patches

    /// <summary>Resonate with chords.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BaseWeaponEnchantmentApplyToPostfix(BaseWeaponEnchantment __instance, Item item)
    {
        if (!__instance.IsForge() || item is not MeleeWeapon weapon || weapon != Game1.player.CurrentTool)
        {
            return;
        }

        foreach (var chord in State.ResonantChords.Values)
        {
            chord.QuenchSingleForge(weapon, __instance);
        }
    }

    #endregion harmony patches
}
