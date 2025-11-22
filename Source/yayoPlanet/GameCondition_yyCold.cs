using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class GameCondition_yyCold : GameCondition
{

    private static int ActiveTick => (Find.TickManager.TicksAbs % GenDate.TicksPerYear) - (GenDate.TicksPerQuadrum * 3);

    private static float ActiveFactor => Mathf.Clamp(Mathf.Max(ActiveTick, 0) / (float)GenDate.TicksPerQuadrum, 0f, 1f);

    private static List<SkyOverlay> Overlays
    {
        get
        {
            var ar = new List<SkyOverlay>();
            if (ActiveTick <= 0)
            {
                return ar;
            }

            SkyOverlay sky = new WeatherOverlay_fastWind();
            sky.SetOverlayColor(new Color(0.6f, 0.6f, 0.9f, 0.25f)); // 바람 색깔
            ar.Add(sky);

            return ar;
        }
    }


    public override float AnimalDensityFactor(Map map)
    { return Mathf.Clamp01(1f - (Mathf.Abs(map.mapTemperature.OutdoorTemp) / 75f)); }


    // 셀 데미지
    public override void DoCellSteadyEffects(IntVec3 cell, Map map)
    {
        var temp = cell.GetTemperature(map);
        if (cell.GetRoof(map) != null || !(temp < -130f))
        {
            return;
        }

        var ar_thing = cell.GetThingList(map);
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < ar_thing.Count; i++)
        {
            var thing = ar_thing[i];
            switch (thing.def.category)
            {
                case ThingCategory.Item:
                    thing.TakeDamage(new DamageInfo(DamageDefOf.Rotting, ActiveFactor * 2.5f));
                    break;
                case ThingCategory.Pawn:
                    if (thing is Pawn p)
                    {
                        if (Rand.Value < Mathf.Clamp01(-(temp - p.SafeTemperatureRange().min) / 100f))
                        {
                            var dmg = 0f;
                            if (p.def.race.FleshType == FleshTypeDefOf.Normal ||
                                p.def.race.FleshType == FleshTypeDefOf.Insectoid)
                            {
                                dmg = 15f;
                            }
                            else if (p.def.race.FleshType == FleshTypeDefOf.Mechanoid)
                            {
                                dmg = 1f;
                            }

                            if (Rand.Value < 0.25f)
                            {
                                thing.TakeDamage(new DamageInfo(DamageDefOf.Frostbite, dmg));
                            }
                        }
                    }

                    break;
            }
        }
    }

    public override void End()
    {
        base.End();
        util.threatMultiply = 1f;
    }


    public override void GameConditionDraw(Map map)
    {
        foreach (var skyOverlay in Overlays)
        {
            skyOverlay.DrawOverlay(map);
        }
    }

    public override void GameConditionTick()
    {
        base.GameConditionTick();

        var affectedMaps = AffectedMaps;
        util.threatMultiply = 1f - (ActiveFactor * YayoPlanetMod.val_threat);

        if (ActiveTick < 0)
        {
            return;
        }

        foreach (var overlay in Overlays)
        {
            foreach (var map in affectedMaps)
            {
                overlay.TickOverlay(map, ActiveFactor);
            }
        }
    }

    public override void Init()
    {
        base.Init();
        Messages.Message("nt_cold".Translate(), MessageTypeDefOf.NeutralEvent);
    }

    public override List<SkyOverlay> SkyOverlays(Map map) { return Overlays; }

    // 하늘 색깔
    public override SkyTarget? SkyTarget(Map map)
    {
        return new SkyTarget(
            0.75f,
            new SkyColorSet(
                new Color(0.2f, 0.2f, 0.85f), //sky
                new Color(0.1f, 0.1f, 0.95f), // shadow
                new Color(0.15f, 0.15f, 0.9f),
                1f),
            1f,
            1f);
    }

    // 하늘 색깔
    public override float SkyTargetLerpFactor(Map map) { return ActiveFactor; }

    // 오프셋 기온
    public override float TemperatureOffset()
    {
        //Log.Message(active_factor.ToString());
        return ActiveFactor * YayoPlanetMod.val_yyCold;
    }

    public override int TransitionTicks => 6000;
}