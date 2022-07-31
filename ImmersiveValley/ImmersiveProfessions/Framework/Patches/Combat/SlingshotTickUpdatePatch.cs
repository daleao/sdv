namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using Sounds;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotTickUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotTickUpdatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.tickUpdate));
    }

    /// <summary>Patch to overcharge Slingshot for Desperado.</summary>
    [HarmonyPostfix]
    internal static void SlingshotTickUpdatePostfix(Slingshot __instance, Farmer who)
    {
        if (!who.HasProfession(Profession.Desperado) || who.CurrentTool != __instance || __instance.attachments[0] is null ||
            !who.usingSlingshot) return;

        who.CanMove = true;
        who.speed = who.HasProfession(Profession.Desperado, true) ? 5 : 2;

        var overcharge = __instance.GetDesperadoOvercharge(who);
        if (overcharge <= 0f) return;

        who.jitterStrength = Math.Max(0f, overcharge - 0.5f);

        if (Game1.soundBank is null) return;

        SFX.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!SFX.SinWave.IsPlaying) SFX.SinWave.Play();

        SFX.SinWave.SetVariable("Pitch", 2400f * overcharge);
    }
}