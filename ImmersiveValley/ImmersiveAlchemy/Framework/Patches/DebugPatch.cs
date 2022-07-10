//using DaLion.Common;
//using DaLion.Common.Harmony;
//using HarmonyLib;
//using JetBrains.Annotations;

//namespace DaLion.Stardew.Alchemy.Framework.Patches;

///// <summary>Wildcard prefix patch for on-demand debugging.</summary>
//[UsedImplicitly]
//internal class DebugPatch : BasePatch
//{
//    /// <summary>Construct an instance.</summary>
//    internal DebugPatch()
//    {
//#if DEBUG
//        Target = RequireMethod<>(nameof(.));
//#endif
//    }

//    #region harmony patches

//    [HarmonyPrefix]
//    private static bool DebugPrefix(object __instance)
//    {
//        Log.D("DebugPatch called!");

//        return true; // run original logic
//    }

//    #endregion harmony patches
//}