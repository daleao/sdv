namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Harmonics.Framework.VirtualProperties;
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
    private static void BaseWeaponEnchantmentApplyToPostfix(BaseWeaponEnchantment __instance, Item item)
    {
        var player = Game1.player;
        if (item is not MeleeWeapon weapon || weapon != player.CurrentTool)
        {
            return;
        }

        var chords = player.Get_ResonatingChords().ToList();
        foreach (var chord in chords)
        {
            chord.QuenchSingleForge(weapon, __instance);
        }
    }

    #endregion harmony patches
}
