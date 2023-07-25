using RimWorld;
using Verse;

namespace yayoPlanet;

public static class util
{
    public static float threatMultiply = 1f;

    public static bool isYayoGC(Map map)
    {
        var result = false;
        foreach (var gc in map.gameConditionManager.ActiveConditions)
        {
            if (gc.def.defName.Contains("yy"))
            {
                result = true;
            }
        }

        return result;
    }

    public static GameConditionDef getRandomGcDef()
    {
        return modBase.ar_gc[Rand.Range(0, modBase.ar_gc.Count)];
    }


    public static void setRandomYayoGc(Map map)
    {
        GameCondition gc;
        if (modBase.bl_randomType)
        {
            gc = GameConditionMaker.MakeConditionPermanent(getRandomGcDef());
        }
        else
        {
            gc = GameConditionMaker.MakeConditionPermanent(
                modBase.ar_gc[
                    ((Find.TickManager.TicksAbs / GenDate.TicksPerYear) - modBase.eventStart) / modBase.eventCycle %
                    modBase.ar_gc.Count]);
        }

        map.gameConditionManager.RegisterCondition(gc);
    }
}