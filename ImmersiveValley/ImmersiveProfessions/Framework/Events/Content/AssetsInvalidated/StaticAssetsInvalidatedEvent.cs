namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticAssetsInvalidatedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars")))
        {
            Textures.BarsTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter")))
        {
            Textures.MeterTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression")))
        {
            Textures.ProgressionTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
        }
    }
}