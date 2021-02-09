namespace TheLion.Common.Integrations
{
	/// <summary>Handles integration with a given mod.</summary>
	internal interface IModIntegration
	{
		/// <summary>A human-readable name for the mod.</summary>
		string Label { get; }

		/// <summary>Whether the mod is available.</summary>
		bool IsLoaded { get; }
	}
}