using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoPlanet;

[HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow))]
internal class StorytellerUtility_DefaultThreatPointsNow
{
    private static void Postfix(ref float __result, IIncidentTarget target)
    {
        if (target is Map map && util.IsYayoGC(map))
        {
            __result *= util.threatMultiply;
        }
    }
}