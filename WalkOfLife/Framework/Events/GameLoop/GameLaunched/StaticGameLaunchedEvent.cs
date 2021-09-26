using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticGameLaunchedEvent : GameLaunchedEvent
	{
		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public override void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// add Generic Mod Config Menu integration
			new Integrations.GenericModConfigMenuIntegrationForAwesomeTools(
				getConfig: () => ModEntry.Config,
				reset: () =>
				{
					ModEntry.Config = new ModConfig();
					ModEntry.ModHelper.WriteConfig(ModEntry.Config);
				},
				saveAndApply: () =>
				{
					ModEntry.ModHelper.WriteConfig(ModEntry.Config);
				},
				log: ModEntry.Log,
				modRegistry: ModEntry.ModHelper.ModRegistry,
				manifest: ModEntry.Manifest
			).Register();
		}
	}
}