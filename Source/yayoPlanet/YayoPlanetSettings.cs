using Verse;

namespace yayoPlanet;

public class YayoPlanetSettings : ModSettings
{
    public bool bl_randomType;

    public bool bl_yyCold = true;

    public bool bl_yyHot = true;

    public bool bl_yyTornado = true;
    public int eventCycle = 2;
    public int eventStart = 1;

    public float val_threat = 0.5f;
    public float val_yyCold = -270f;
    public float val_yyHot = 200f;
    public float val_yyTornado = 1f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref eventStart, "eventStart", 1);
        Scribe_Values.Look(ref eventCycle, "eventCycle", 2);

        Scribe_Values.Look(ref bl_randomType, "bl_randomType", false);

        Scribe_Values.Look(ref bl_yyCold, "bl_yyCold", true);
        Scribe_Values.Look(ref val_yyCold, "val_yyCold", -270f);

        Scribe_Values.Look(ref bl_yyHot, "bl_yyHot", true);
        Scribe_Values.Look(ref val_yyHot, "val_yyHot", 200f);

        Scribe_Values.Look(ref bl_yyTornado, "bl_yyTornado", true);
        Scribe_Values.Look(ref val_yyTornado, "val_yyTornado", 1f);

        Scribe_Values.Look(ref val_threat, "val_threat", 0.5f);
    }
}
