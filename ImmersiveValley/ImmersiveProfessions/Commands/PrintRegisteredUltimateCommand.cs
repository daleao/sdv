namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions;

#endregion using directives

internal class PrintRegisteredUltimateCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "which_ult";

    /// <inheritdoc />
    public string Documentation => "Print the currently registered Special Ability.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Log.I("Not registered to an Ultimate.");
            return;
        }

        var key = ModEntry.PlayerState.RegisteredUltimate.Index.ToString().SplitCamelCase()[0].ToLowerInvariant();
        var professionDisplayName = ModEntry.i18n.Get(key + ".name.male");
        var ultiName = ModEntry.i18n.Get(key + ".ulti");
        Log.I($"Registered to {professionDisplayName}'s {ultiName}.");
    }
}