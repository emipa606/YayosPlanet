using RimWorld;
using Verse;

namespace yayoPlanet;

public static class util
{
    public static float threatMultiply = 1f;

    private static GameConditionDef getRandomGcDef()
    { return YayoPlanetMod.ar_gc[Rand.Range(0, YayoPlanetMod.ar_gc.Count)]; }

    public static bool IsYayoGC(Map map)
    {
        var result = false;
        foreach(var gc in map.gameConditionManager.ActiveConditions)
        {
            if(gc.def.defName.Contains("yy"))
            {
                result = true;
            }
        }

        return result;
    }


    public static void SetRandomYayoGc(Map map)
    {
        GameCondition gc;
        if(YayoPlanetMod.bl_randomType)
        {
            gc = GameConditionMaker.MakeConditionPermanent(getRandomGcDef());
        } else
        {
            gc = GameConditionMaker.MakeConditionPermanent(
                YayoPlanetMod.ar_gc[
                    ((Find.TickManager.TicksAbs / GenDate.TicksPerYear) - YayoPlanetMod.eventStart) /
                    YayoPlanetMod.eventCycle %
                    YayoPlanetMod.ar_gc.Count]);
        }

        map.gameConditionManager.RegisterCondition(gc);
    }
}