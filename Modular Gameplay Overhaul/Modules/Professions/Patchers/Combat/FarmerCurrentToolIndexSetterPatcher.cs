namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
            (slingshot.numAttachmentSlots.Value < 2 || slingshot.attachments.Length < 2))
        {
            slingshot.numAttachmentSlots.Value = 2;
            slingshot.attachments.SetCount(2);
        }
        else if (!__instance.HasProfession(Profession.Rascal) && slingshot.numAttachmentSlots.Value >= 2)
        {
            var item = slingshot.attachments[1];
            slingshot.numAttachmentSlots.Value = 1;
            //slingshot.attachments.SetCount(1); this cannot be used to reduce the array size
            if (item is not null && !__instance.addItemToInventoryBool(item))
            {
                Game1.createItemDebris(
                    item,
                    __instance.getStandingPosition(),
                    1,
                    __instance.currentLocation);
            }
        }
    }

    #endregion harmony patches
}
