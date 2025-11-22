using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class GameCondition_yyTornado : GameCondition
{

    private int nextTick;
    // 하늘 색깔
    private readonly SkyColorSet skyColors = new(
        new Color(0.7f, 0.4f, 0.25f), //sky
        new Color(0.6f, 0.25f, 0.15f), // shadow
        new Color(0.7f, 0.4f, 0.25f),
        1f); // overay


    private void Spawn(Map map)
    {
        if (!tryFindCell(out var intVec, map))
        {
            return;
        }

        GenSpawn.Spawn(ThingDef.Named("yy_Tornado"), intVec, map);
    }


    private static bool tryFindCell(out IntVec3 cell, Map map)
    {
        var maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
        return CellFinderLoose.TryFindSkyfallerCell(
            ThingDefOf.MeteoriteIncoming,
            map,
            TerrainAffordanceDefOf.Walkable,
            out cell,
            10,
            default,
            -1,
            false,
            false,
            false,
            false,
            true,
            true,
            delegate (IntVec3 x)
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

    private static int ActiveTick => (Find.TickManager.TicksAbs % GenDate.TicksPerYear) - (GenDate.TicksPerQuadrum * 3);

    private static float ActiveFactor => Mathf.Clamp(
        Mathf.Max(ActiveTick - GenDate.TicksPerDay, 0) / (float)GenDate.TicksPerQuadrum,
        0f,
        1f);

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
            sky.SetOverlayColor(new Color(0.7f, 0.3f, 0.2f, 0.7f * ActiveFactor));
            ar.Add(sky);

            return ar;
        }
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


    public override void GameConditionDraw(Map map)
    {
        foreach (var overlay in Overlays)
        {
            overlay.DrawOverlay(map);
        }
    }

    public override void GameConditionTick()
    {
        util.threatMultiply = 1f - (ActiveFactor * YayoPlanetMod.val_threat);

        var affectedMaps = AffectedMaps;

        if (ActiveTick >= 0 &&
            Find.TickManager.TicksAbs >= nextTick &&
            ActiveTick <= GenDate.TicksPerQuadrum - GenDate.TicksPerDay)
        {
            nextTick = Find.TickManager.TicksAbs + (int)(500 + (GenDate.TicksPerDay * 0.5f * (1f - ActiveFactor)));

            foreach (var map in affectedMaps)
            {
                Spawn(map);
            }
        }

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
        Messages.Message("nt_tornado".Translate(), MessageTypeDefOf.NeutralEvent);
    }

    public override List<SkyOverlay> SkyOverlays(Map map) { return Overlays; }

    public override SkyTarget? SkyTarget(Map map) { return new SkyTarget(1f, skyColors, 1f, 1f); }

    // 하늘 색깔
    public override float SkyTargetLerpFactor(Map map) { return Mathf.Clamp(ActiveFactor * 1.5f, 0f, 1f); }

    public override int TransitionTicks => 6000;
}