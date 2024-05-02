namespace DaLion.Professions.Framework.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The random tweak settings for PRFS.</summary>
public sealed class TweexConfig
{
    private float _cropWitherChance = 0f;
    private bool _immesriveHeavyTapperYield = true;

    #region farming

    /// <summary>Gets the chance a crop may wither per day left un-watered.</summary>
    [JsonProperty]
    [GMCMSection("prfs.farming_twx")]
    [GMCMPriority(0)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.05f)]
    public float CropWitherChance
    {
        get => this._cropWitherChance;
        internal set
        {
            this._cropWitherChance = Math.Clamp(value, 0f, 1f);
        }
    }

    #endregion farming

    #region foraging



  

    #endregion foraging
}
