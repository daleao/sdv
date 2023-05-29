namespace DaLion.Overhaul.Modules.Slingshots.Extensions;

#region using directives

using System.Diagnostics;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    private const int SlingshotCooldown = 2000;

    /// <summary>Gets the total firing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="slingshot">The <paramref name="farmer"/>'s slingshot.</param>
    /// <returns>The total firing speed modifier, a number between 0 and 1.</returns>
    internal static float GetTotalFiringSpeedModifier(this Farmer farmer, Slingshot? slingshot = null)
    {
        slingshot ??= farmer.CurrentTool as Slingshot;

        var modifier = 1f;
        if (slingshot is not null)
        {
            modifier *= slingshot.Get_EmeraldFireSpeed();
        }

        modifier *= 1f / (1f + farmer.weaponSpeedModifier);
        return modifier;
    }

    /// <summary>Determines whether the <paramref name="farmer"/> is stepping on a snowy tile.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the corresponding <see cref="FarmerSprite"/> is using snowy step sounds, otherwise <see langword="false"/>.</returns>
    internal static bool IsSteppingOnSnow(this Farmer farmer)
    {
        return farmer.FarmerSprite.currentStep == "snowyStep";
    }

    [Conditional("RELEASE")]
    internal static void DoSlingshotSpecialCooldown(this Farmer user, Slingshot? slingshot = null)
    {
        slingshot ??= (Slingshot)user.CurrentTool;

        if (slingshot.Get_IsOnSpecial())
        {
            SlingshotsModule.State.SlingshotCooldown = SlingshotCooldown;
            if (!ProfessionsModule.ShouldEnable && user.professions.Contains(Farmer.acrobat))
            {
                SlingshotsModule.State.SlingshotCooldown /= 2;
            }

            if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>())
            {
                SlingshotsModule.State.SlingshotCooldown /= 2;
            }

            SlingshotsModule.State.SlingshotCooldown = (int)(SlingshotsModule.State.SlingshotCooldown *
                                                             slingshot.Get_GarnetCooldownReduction() *
                                                             user.Get_CooldownReduction());
        }
        else
        {
            SlingshotsModule.State.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (SlingshotsModule.State.SlingshotCooldown <= 0)
            {
                Game1.playSound("objectiveComplete");
            }
        }
    }
}
