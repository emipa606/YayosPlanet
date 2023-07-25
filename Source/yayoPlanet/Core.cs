using RimWorld;
using Verse;

namespace yayoPlanet;

public class Core : MapComponent
{
    private readonly int notice_tick = GenDate.TicksPerQuadrum;

    public Core(Map map) : base(map)
    {
    }

    public override void MapComponentTick()
    {
        // 매 틱마다
        base.MapComponentTick();
        modBase.tickOfYear = Find.TickManager.TicksAbs % GenDate.TicksPerYear;

        // 이벤트 종료
        if (modBase.tickOfYear == 0)
        {
            for (var i = map.gameConditionManager.ActiveConditions.Count - 1; i >= 0; i--)
            {
                map.gameConditionManager.ActiveConditions[i].End();
            }
        }

        // 이벤트 예고
        if (modBase.tickOfYear != notice_tick)
        {
            return;
        }

        var passedYearAbs = Find.TickManager.TicksAbs / GenDate.TicksPerYear;
        if (!util.isYayoGC(map)
            && passedYearAbs >= modBase.eventStart
            && (passedYearAbs - modBase.eventStart) % modBase.eventCycle == 0)
        {
            util.setRandomYayoGc(map);
        }
    }
}