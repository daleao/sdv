namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Set Rascal ammo slots.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, int value)
    {
        if (value < 0 || value > __instance.Items.Count || __instance.Items[value] is not Slingshot slingshot)
        {
            return;
        }

        if (__instance.HasProfession(Profession.Rascal) &&
            (slingshot.numAttachmentSlots.Value == 1 || slingshot.attachments.Length == 1))
        {
            slingshot.numAttachmentSlots.Value = 2;
            slingshot.attachments.SetCount(2);
        }
        else if (!__instance.HasProfession(Profession.Rascal) && (slingshot.numAttachmentSlots.Value == 2 || slingshot.attachments.Length == 2))
        {
            var replacement = ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is { } bowData
                ? (Slingshot)ArcheryIntegration.Instance.ModApi.CreateWeapon(Manifest, bowData.WeaponId)
                : new Slingshot(slingshot.InitialParentTileIndex);

            if (slingshot.attachments[0] is { } ammo1)
            {
                replacement.attachments[0] = (SObject)ammo1.getOne();
                replacement.attachments[0].Stack = ammo1.Stack;
            }

            if (slingshot.attachments.Length > 1 && slingshot.attachments[1] is { } ammo2)
            {
                var drop = (SObject)ammo2.getOne();
                drop.Stack = ammo2.Stack;
                if (!__instance.addItemToInventoryBool(drop))
                {
                    Game1.createItemDebris(drop, __instance.getStandingPosition(), -1, __instance.currentLocation);
                }
            }

            __instance.Items[value] = replacement;
        }
    }

    #endregion harmony patches
}
