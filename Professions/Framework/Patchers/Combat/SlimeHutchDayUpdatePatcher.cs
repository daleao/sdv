namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlimeHutchDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SlimeHutch>(nameof(SlimeHutch.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to color Slime Balls.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SlimeHutchDayUpdatePostfix(SlimeHutch __instance)
    {
        var owner = __instance.ParentBuilding.GetOwner();
        if (!owner.HasProfessionOrLax(Profession.Piper))
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        var slimes = __instance.characters.OfType<GreenSlime>().ToArray();
        foreach (var @object in __instance.Objects.Values)
        {
            if (@object.QualifiedItemId == QIDs.SlimeBall)
            {
                @object.orderData.Set(getSlimeString(slimes.Choose(r)));
            }
        }

        if (!owner.HasProfessionOrLax(Profession.Piper, true) || __instance.waterSpots.Length < 6)
        {
            return;
        }

        foreach (var @object in __instance.objects.Values)
        {
            if (!@object.IsSprinkler())
            {
                continue;
            }

            foreach (var v in @object.GetSprinklerTiles())
            {
                if (v is { X: 16f, Y: >= 5f and <= 10f })
                {
                    __instance.waterSpots[(int)v.Y - 5] = true;
                }
            }
        }

        return;

        string getSlimeString(GreenSlime slime)
        {
            var color = slime.color.Value;
            var isTiger = slime.Name == "Tiger Slime";
            return $"{color.PackedValue}/{isTiger}/{slime.firstGeneration.Value}/{slime.specialNumber.Value}";
        }
    }

    #endregion harmony patches
}
