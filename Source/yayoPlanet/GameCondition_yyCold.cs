using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class GameCondition_yyCold : GameCondition
{
    public override int TransitionTicks => 6000;

    private int active_tick => (Find.TickManager.TicksAbs % GenDate.TicksPerYear) - (GenDate.TicksPerQuadrum * 3);

    private float active_factor => Mathf.Clamp(Mathf.Max(active_tick, 0) / (float)GenDate.TicksPerQuadrum, 0f, 1f);

    private List<SkyOverlay> overlays
    {
        get
        {
            var ar = new List<SkyOverlay>();
            if (active_tick <= 0)
            {
                return ar;
            }

            SkyOverlay sky = new WeatherOverlay_fastWind();
            sky.OverlayColor = new Color(0.6f, 0.6f, 0.9f, 0.25f); // 바람 색깔
            ar.Add(sky);

            return ar;
        }
    }

    public override void Init()
    {
        base.Init();
        Messages.Message("nt_cold".Translate(), MessageTypeDefOf.NeutralEvent);
    }

    public override void End()
    {
        base.End();
        util.threatMultiply = 1f;
    }

    public override void GameConditionTick()
    {
        base.GameConditionTick();

        var affectedMaps = AffectedMaps;
        util.threatMultiply = 1f - (active_factor * modBase.val_threrat);

        if (active_tick < 0)
        {
            return;
        }

        foreach (var overlay in overlays)
        {
            foreach (var map in affectedMaps)
            {
                overlay.TickOverlay(map);
            }
        }
    }

    // 오프셋 기온
    public override float TemperatureOffset()
    {
        //Log.Message(active_factor.ToString());
        return active_factor * modBase.val_yyCold;
    }

    // 하늘 색깔
    public override SkyTarget? SkyTarget(Map map)
    {
        return new SkyTarget(0.75f, new SkyColorSet(
            new Color(0.2f, 0.2f, 0.85f), //sky
            new Color(0.1f, 0.1f, 0.95f), // shadow
            new Color(0.15f, 0.15f, 0.9f), 1f), 1f, 1f);
    }

    // 하늘 색깔
    public override float SkyTargetLerpFactor(Map map)
    {
        return active_factor;
    }


    public override float AnimalDensityFactor(Map map)
    {
        return Mathf.Clamp01(1f - (Mathf.Abs(map.mapTemperature.OutdoorTemp) / 75f));
    }


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
                    thing.TakeDamage(new DamageInfo(DamageDefOf.Rotting, active_factor * 2.5f));
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


    public override void GameConditionDraw(Map map)
    {
        foreach (var skyOverlay in overlays)
        {
            skyOverlay.DrawOverlay(map);
        }
    }

    public override List<SkyOverlay> SkyOverlays(Map map)
    {
        return overlays;
    }
}