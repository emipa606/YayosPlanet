using RimWorld;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class GameCondition_yyHot : GameCondition
{
    public override int TransitionTicks => 6000;


    private static int ActiveTick => (Find.TickManager.TicksAbs % GenDate.TicksPerYear) - (GenDate.TicksPerQuadrum * 3);

    private static float ActiveFactor => Mathf.Clamp(Mathf.Max(ActiveTick, 0) / (float)GenDate.TicksPerQuadrum, 0f, 1f);

    public override void Init()
    {
        base.Init();
        Messages.Message("nt_hot".Translate(), MessageTypeDefOf.NeutralEvent);
    }

    public override void End()
    {
        base.End();
        util.threatMultiply = 1f;
    }

    public override void GameConditionTick()
    {
        base.GameConditionTick();
        util.threatMultiply = 1f - (ActiveFactor * YayoPlanetMod.val_threat);
    }

    // 오프셋 기온
    public override float TemperatureOffset()
    {
        return ActiveFactor * YayoPlanetMod.val_yyHot;
    }

    // 하늘 색깔
    public override SkyTarget? SkyTarget(Map map)
    {
        return new SkyTarget(1f, new SkyColorSet(
            new Color(0.8f, 0.25f, 0.25f), //sky
            new Color(0.95f, 0.15f, 0.15f), // shadow
            new Color(0.85f, 0.25f, 0.25f), 1f), 1f, 1f);
    }

    // 하늘 색깔
    public override float SkyTargetLerpFactor(Map map)
    {
        return ActiveFactor;
    }


    public override float AnimalDensityFactor(Map map)
    {
        return Mathf.Clamp01(1f - (Mathf.Abs(map.mapTemperature.OutdoorTemp) / 65f));
    }
}