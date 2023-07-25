using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class GameCondition_yyTornado : GameCondition
{
    // 하늘 색깔
    private readonly SkyColorSet SkyColors = new SkyColorSet(
        new Color(0.7f, 0.4f, 0.25f), //sky
        new Color(0.6f, 0.25f, 0.15f), // shadow
        new Color(0.7f, 0.4f, 0.25f), 1f); // overay

    private int nextTick;

    public override int TransitionTicks => 6000;

    private int active_tick => (Find.TickManager.TicksAbs % GenDate.TicksPerYear) - (GenDate.TicksPerQuadrum * 3);

    private float active_factor =>
        Mathf.Clamp(Mathf.Max(active_tick - GenDate.TicksPerDay, 0) / (float)GenDate.TicksPerQuadrum, 0f, 1f);

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
            sky.OverlayColor = new Color(0.7f, 0.3f, 0.2f, 0.7f * active_factor);
            ar.Add(sky);

            return ar;
        }
    }

    public override void Init()
    {
        base.Init();
        Messages.Message("nt_tornado".Translate(), MessageTypeDefOf.NeutralEvent);
    }

    public override void End()
    {
        base.End();
        util.threatMultiply = 1f;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref nextTick, "nextTick");
    }

    public override void GameConditionTick()
    {
        util.threatMultiply = 1f - (active_factor * modBase.val_threrat);

        var affectedMaps = AffectedMaps;

        if (active_tick >= 0 && Find.TickManager.TicksAbs >= nextTick &&
            active_tick <= GenDate.TicksPerQuadrum - GenDate.TicksPerDay)
        {
            nextTick = Find.TickManager.TicksAbs + (int)(500 + (GenDate.TicksPerDay * 0.5f * (1f - active_factor)));

            foreach (var map in affectedMaps)
            {
                Spawn(map);
            }
        }

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


    public override void GameConditionDraw(Map map)
    {
        foreach (var overlay in overlays)
        {
            overlay.DrawOverlay(map);
        }
    }

    public override List<SkyOverlay> SkyOverlays(Map map)
    {
        return overlays;
    }

    public override SkyTarget? SkyTarget(Map map)
    {
        return new SkyTarget(1f, SkyColors, 1f, 1f);
    }

    // 하늘 색깔
    public override float SkyTargetLerpFactor(Map map)
    {
        return Mathf.Clamp(active_factor * 1.5f, 0f, 1f);
    }


    private void Spawn(Map map)
    {
        if (!TryFindCell(out var intVec, map))
        {
            return;
        }

        GenSpawn.Spawn(ThingDef.Named("yy_Tornado"), intVec, map);
    }


    private bool TryFindCell(out IntVec3 cell, Map map)
    {
        var maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
        return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default, -1, false,
            false, false, false, true, true, delegate(IntVec3 x)
            {
                var num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
                var cellRect = CellRect.CenteredOn(x, num, num);
                var num2 = 0;
                foreach (var c in cellRect)
                {
                    if (c.InBounds(map) && c.Standable(map))
                    {
                        num2++;
                    }
                }

                return num2 >= maxMineables;
            });
    }
}