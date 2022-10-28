namespace DaLion.Redux.Professions.Patches.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
internal sealed class TrackedItemCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TrackedItemCtorPatch"/> class.</summary>
    internal TrackedItemCtorPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.TrackedItem".ToType()
            .RequireConstructor(typeof(Item), typeof(Action<Item>), typeof(Action<Item>));
    }

    #region harmony patches

    /// <summary>Patch to fix collected rings from crab pots.</summary>
    [HarmonyPrefix]
    private static void TrackedItemCtorPrefix(ref Item item)
    {
        if (!item.ParentSheetIndex.IsIn(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
        {
            return;
        }

        item = item.ParentSheetIndex switch
        {
            14 or 51 => new MeleeWeapon(item.ParentSheetIndex),
            _ => new Ring(item.ParentSheetIndex),
        };
    }

    #endregion harmony patches
}
