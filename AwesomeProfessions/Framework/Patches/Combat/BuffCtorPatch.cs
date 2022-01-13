using JetBrains.Annotations;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

[UsedImplicitly]
internal class BuffCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BuffCtorPatch()
    {
        Original = RequireConstructor<Buff>(typeof(int));
    }

    /// <summary>Patch to change Slimed debuff into Slimed buff for prestiged Piper.</summary>
    private static void BuffCtorPostfix(Buff __instance, int which)
    {
        if (which != 13 || !Game1.player.HasPrestigedProfession("Piper")) return;
        __instance.buffAttributes[9] = -__instance.buffAttributes[9];
    }
}