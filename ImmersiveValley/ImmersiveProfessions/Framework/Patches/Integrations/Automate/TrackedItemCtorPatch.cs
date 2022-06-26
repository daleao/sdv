#nullable enable
namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackedItemCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Action<object, Item>? _SetItem;

    /// <summary>Construct an instance.</summary>
    internal TrackedItemCtorPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.TrackedItem".ToType()
                .RequireConstructor(new[] { typeof(Item), typeof(Action<Item>), typeof(Action<Item>) });
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to fix collected rings from crab pots.</summary>
    [HarmonyPostfix]
    private static void TrackedItemCtorPostfix(object __instance, Item item)
    {
        if (!item.ParentSheetIndex.IsIn(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532,
                533, 534)) return;

        _SetItem ??= "Pathoschild.Stardew.Automate.TrackedItem".ToType().RequireField("Item")
            .CompileUnboundFieldSetterDelegate<Action<object, Item>>();
        if (item.ParentSheetIndex.IsIn(14, 51))
            _SetItem(__instance, new MeleeWeapon(item.ParentSheetIndex));
        else
            _SetItem(__instance, new Ring(item.ParentSheetIndex));
    }

    #endregion harmony patches
}