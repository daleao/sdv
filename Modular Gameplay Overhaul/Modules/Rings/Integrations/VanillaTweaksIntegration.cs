namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

internal sealed class VanillaTweaksIntegration : BaseIntegration
{
    /// <summary>Initializes a new instance of the <see cref="VanillaTweaksIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal VanillaTweaksIntegration(IModRegistry modRegistry)
        : base("VanillaTweaks", "Taiyo.VanillaTweaks", null, modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the value of the <c>RingsCategoryEnabled</c> config setting.</summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Doesn't make sense in this context.")]
    internal static bool RingsCategoryEnabled { get; private set; }

    /// <inheritdoc />
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        RingsCategoryEnabled = ModHelper
            .ReadContentPackConfig("Taiyo.VanillaTweaks")?
            .Value<bool>("RingsCategoryEnabled") == true;
        ModHelper.GameContent.InvalidateCache("Maps/springobjects");
    }
}
