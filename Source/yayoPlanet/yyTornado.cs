using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace yayoPlanet;

[StaticConstructorOnStartup]
public class yyTornado : ThingWithComps
{
    private const float AnimalPawnDamageFactor = 0.75f;

    private const float BaseDamage = 30f;

    private const float BuildingDamageFactor = 0.8f;

    private const float CellsPerSecond = 1.7f;

    private const int CloseDamageIntervalTicks = 15;

    private const float CloseDamageRadius = 4.2f;

    private const float DirectionChangeSpeed = 0.78f;

    private const float DirectionNoiseFrequency = 0.002f;

    private const float DownedPawnDamageFactor = 0.2f;

    private const int FadeInTicks = 120;

    private const int FadeOutTicks = 120;

    private const float FarDamageMTBTicks = 15f;

    private const float FarDamageRadius = 10f;

    private const float ItemDamageFactor = 0.68f;

    private const int MaxDestroyHp = 1000;
    private const int MaxHp = 30;

    private const float MaxMidOffset = 2f;

    private const float PlantDamageFactor = 1.7f;

    private const int RoofDestructionIntervalTicks = 20;

    private const int SpawnMoteEveryTicks = 4;

    private const float ThreeDimensionalEffectStrength = 4f;

    private const float TornadoAnimationSpeed = 25f;
    private const float Wind = 5f;

    private static ModuleBase directionNoise;

    private static readonly MaterialPropertyBlock matPropertyBlock = new();

    private static readonly FloatRange PartsDistanceFromCenter = new(1f, FarDamageRadius);

    private static readonly List<Thing> tmpThings = [];

    private static readonly Material TornadoMaterial = MaterialPool.MatFrom(
        "Things/Ethereal/Tornado",
        ShaderDatabase.Transparent,
        MapMaterialRenderQueues.Tornado);

    private static readonly float ZOffsetBias = -4f * PartsDistanceFromCenter.min;
    private int _hp = 999;
    private int destroyHp = 99999;

    private float direction;

    private int leftFadeOutTicks = -1;

    private Vector2 realPosition;

    private readonly List<IntVec3> removedRoofsTmp = [];

    private int spawnTick;

    private Sustainer sustainer;

    private int ticksLeftToDisappear = -1;

    private static float adjustedDistanceFromCenter(float distanceFromCenter)
    {
        var num = Mathf.Min(distanceFromCenter / 8f, 1f);
        num *= num;
        return distanceFromCenter * num;
    }

    private bool cellImmuneToDamage(IntVec3 c)
    {
        if(c.Roofed(Map) && c.GetRoof(Map).isThickRoof)
        {
            return true;
        }

        var edifice = c.GetEdifice(Map);
        return edifice != null &&
            edifice.def.category == ThingCategory.Building &&
            (edifice.def.building.isNaturalRock || edifice.def == ThingDefOf.Wall && edifice.Faction == null);
    }

    private void changeDirection(int dmg)
    {
        // 체력감소에 따라 방향전환
        Hp -= dmg;
        if(Hp > 0)
        {
            return;
        }

        Hp = MaxHp;
        direction = Rand.Range(0f, 360f);
    }

    private void createSustainer()
    {
        LongEventHandler.ExecuteWhenFinished(
            delegate
            {
                var tornado = SoundDefOf.Tornado;
                sustainer = tornado.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
                updateSustainerVolume();
            });
    }

    private void damageCloseThings()
    {
        const float maxDist = 9f;
        var num = GenRadial.NumCellsInRadius(maxDist); // 공격범위
        for(var i = 0; i < num; i++)
        {
            var intVec = Position + GenRadial.RadialPattern[i];

            if(!intVec.InBounds(Map) || cellImmuneToDamage(intVec))
            {
                continue;
            }

            var firstPawn = intVec.GetFirstPawn(Map);
            if(firstPawn is { Downed: true } && Rand.Bool)
            {
                continue;
            }

            var checkCell = intVec.RandomAdjacentCell8Way();

            var room = checkCell.GetRoomOrAdjacent(Map, RegionType.Set_All);
            if(!checkCell.Walkable(Map) || room is { PsychologicallyOutdoors: false })
            {
                continue;
            }

            var dist = intVec.DistanceTo(Position);
            var damageFactor = GenMath.LerpDouble(0f, 8.4f, maxDist, 0.4f, dist);

            doDamage(intVec, damageFactor);
        }
    }

    private void damageFarThings()
    {
        var c = (from x in GenRadial.RadialCellsAround(Position, FarDamageRadius, true) where x.InBounds(Map) select x).RandomElement(
            );
        if(cellImmuneToDamage(c))
        {
            return;
        }

        doDamage(c, 0.5f);
    }

    private void doDamage(IntVec3 c, float damageFactor)
    {
        // 데미지 조절
        damageFactor *= 0.16f * YayoPlanetMod.val_yyTornado;

        tmpThings.Clear();

        tmpThings.AddRange(c.GetThingList(Map));
        var vector = c.ToVector3Shifted();
        var b = new Vector2(vector.x, vector.z);
        var angle = -realPosition.AngleTo(b) + 180f;
        // ReSharper disable once ForCanBeConvertedToForeach
        for(var i = 0; i < tmpThings.Count; i++)
        {
            var thing = tmpThings[i];


            BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
            switch(thing.def.category)
            {
                case ThingCategory.Pawn:
                {
                    var pawn = (Pawn)tmpThings[i];
                    battleLogEntry_DamageTaken =
                            new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Tornado);
                    Find.BattleLog.Add(battleLogEntry_DamageTaken);
                    if(pawn.RaceProps.baseHealthScale < 1f)
                    {
                        damageFactor *= pawn.RaceProps.baseHealthScale * ThreeDimensionalEffectStrength;
                    }

                    if(pawn.RaceProps.Animal)
                    {
                        damageFactor *= 3f;
                    }

                    if(pawn.Downed)
                    {
                        damageFactor *= BuildingDamageFactor;
                    }

                    break;
                }
                case ThingCategory.Item:
                    damageFactor *= ItemDamageFactor;
                    break;
                case ThingCategory.Building:
                    damageFactor *= 0.1f;
                    destroyHp--;
                    if(destroyHp <= 0)
                    {
                        leftFadeOutTicks = FadeOutTicks;
                    }

                    break;
                case ThingCategory.Plant:
                    damageFactor *= 0.025f;
                    break;
            }

            var num = Mathf.Max(GenMath.RoundRandom(BaseDamage * damageFactor), 0);
            if(thing.def.category == ThingCategory.Building)
            {
                num = GenMath.RoundRandom(
                    Mathf.Max(num, thing.def.BaseMaxHitPoints * 0.02f * YayoPlanetMod.val_yyTornado)); // 체력에 따른 퍼뎀
            }

            if(num <= 0)
            {
                continue;
            }

            if(Rand.Bool)
            {
                tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.Blunt, num, 0f, angle, this))
                    .AssociateWithLog(battleLogEntry_DamageTaken);
            } else
            {
                tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.Scratch, num, 0f, angle, this))
                    .AssociateWithLog(battleLogEntry_DamageTaken);
            }
        }

        tmpThings.Clear();
    }

    private void drawTornadoPart(float distanceFromCenter, float initialAngle, float speedMultiplier)
    {
        var ticksGame = Find.TickManager.TicksGame;
        var num = 1f / distanceFromCenter;
        var num2 = TornadoAnimationSpeed * speedMultiplier * num;
        var num3 = (initialAngle + (ticksGame * num2)) % 360f;
        var vector = realPosition.Moved(num3, adjustedDistanceFromCenter(distanceFromCenter));
        vector.y += distanceFromCenter * 1.5f; // 높이
        vector.y += ZOffsetBias;
        var a = new Vector3(
            vector.x,
            AltitudeLayer.Weather.AltitudeFor() + (0.0454545468f * Rand.Range(0f, 1f)),
            vector.y);
        var num4 = (distanceFromCenter * 1f) + 18f; // 크기
        var num5 = 1f;
        switch(num3)
        {
            case > 270f:
                num5 = GenMath.LerpDouble(270f, 360f, 0f, 1f, num3);
                break;
            case > 180f:
                num5 = GenMath.LerpDouble(180f, 270f, 1f, 0f, num3);
                break;
        }

        var num6 = Mathf.Min(distanceFromCenter / (PartsDistanceFromCenter.max + MaxMidOffset), 1f);
        var d = Mathf.InverseLerp(0.18f, 0.4f, num6);
        var a2 = new Vector3(Mathf.Sin((ticksGame / 1000f) + (thingIDNumber * 10)) * 2f, 0f, 0f);
        var pos = a + (a2 * d);
        var a3 = Mathf.Max(1f - num6, 0f) * num5 * FadeInOutFactor * 1f; // 투명도
        var value = new Color(0.27f, 0.1f, 0.05f, a3); // 색깔
        matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
        var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, num3, 0f), new Vector3(num4, 1f, num4));
        Graphics.DrawMesh(MeshPool.plane10, matrix, TornadoMaterial, 0, null, 0, matPropertyBlock); // 메쉬
    }

    private void updateSustainerVolume()
    {
        sustainer.info.volumeFactor = FadeInOutFactor * 0.4f; // 토네이도 볼륨
    }

    private int Hp
    {
        get => _hp;
        set
        {
            if(value > MaxHp)
            {
                _hp = MaxHp;
            } else if(value < 0)
            {
                _hp = 0;
            } else
            {
                _hp = value;
            }
        }
    }

    private float FadeInOutFactor
    {
        get
        {
            var a = Mathf.Clamp01((Find.TickManager.TicksGame - spawnTick) / 120f);
            var b = leftFadeOutTicks < 0 ? 1f : Mathf.Min(leftFadeOutTicks / 120f, 1f);
            return Mathf.Min(a, b);
        }
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        Rand.PushState();
        Rand.Seed = thingIDNumber;

        var makeNum = 30;
        for(var i = 0; i < makeNum; i++)
        {
            // 높이
            drawTornadoPart(PartsDistanceFromCenter.RandomInRange, Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f));
        }

        Rand.PopState();
    }

    protected override void Tick()
    {
        Hp++;
        if(!Spawned)
        {
            return;
        }

        if(sustainer == null)
        {
            createSustainer();
        }

        sustainer?.Maintain();
        updateSustainerVolume();
        GetComp<CompWindSource>().wind = Wind * FadeInOutFactor;
        if(leftFadeOutTicks > 0)
        {
            leftFadeOutTicks--;
            if(leftFadeOutTicks == 0)
            {
                Destroy();
            }
        } else
        {
            directionNoise ??= new Perlin(0.0020000000949949026, 2.0, 0.5, 4, 1948573612, QualityMode.Medium);

            direction +=
                (float)directionNoise.GetValue(Find.TickManager.TicksAbs, thingIDNumber % 500 * 1000f, 0.0) * DirectionChangeSpeed;
            realPosition = realPosition.Moved(direction, 0.0283333343f); // 이동속도
            var intVec = new Vector3(realPosition.x, 0f, realPosition.y).ToIntVec3();

            if(!intVec.CloseToEdge(Map, 5))
            {
                // 현재 셀이 이동불가 셀일경우 방향전환
                if(intVec.Impassable(Map))
                {
                    changeDirection(CloseDamageIntervalTicks);
                }

                Position = intVec;
                if(this.IsHashIntervalTick(120))
                {
                    damageCloseThings();
                }

                if(Rand.MTBEventOccurs(FarDamageMTBTicks, 1f, 0.25f))
                {
                    damageFarThings();
                }

                if(ticksLeftToDisappear > 0)
                {
                    ticksLeftToDisappear--;
                    if(ticksLeftToDisappear == 0)
                    {
                        leftFadeOutTicks = FadeOutTicks;
                    }
                }

            } else
            {
                // 맵의 외각일경우 중앙으로 향하기
                var b = Map.Center.ToVector3();
                direction = realPosition.AngleTo(new Vector2(b.x, b.z));
            }
        }
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref realPosition, "realPosition");
        Scribe_Values.Look(ref direction, "direction");
        Scribe_Values.Look(ref spawnTick, "spawnTick");
        Scribe_Values.Look(ref leftFadeOutTicks, "leftFadeOutTicks");
        Scribe_Values.Look(ref ticksLeftToDisappear, "ticksLeftToDisappear");
        Scribe_Values.Look(ref _hp, "_hp", MaxHp);
        Scribe_Values.Look(ref destroyHp, "destroyHp", MaxDestroyHp);
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        Hp = MaxHp;
        destroyHp = MaxDestroyHp;
        base.SpawnSetup(map, respawningAfterLoad);
        if(!respawningAfterLoad)
        {
            var vector = Position.ToVector3Shifted();
            realPosition = new Vector2(vector.x, vector.z);
            direction = Rand.Range(0f, 360f);
            spawnTick = Find.TickManager.TicksGame;
            leftFadeOutTicks = -1;
            ticksLeftToDisappear = new IntRange(50000, 70000).RandomInRange;
        }

        createSustainer();
    }
}