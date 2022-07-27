namespace DaLion.Common.Integrations.SpaceCore;

#region using directives

using StardewValley;
using System;
using System.Reflection;

#endregion using directives

public interface ISpaceCoreAPI
{
    string[] GetCustomSkills();

    int GetLevelForCustomSkill(Farmer farmer, string skill);

    void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);

    int GetProfessionId(string skill, string profession);

    void AddEventCommand(string command, MethodInfo info);

    void RegisterSerializerType(Type type);

    void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);

    void RegisterCustomLocationContext(string name, Func<Random, LocationWeather> getLocationWeatherForTomorrowFunc);
}