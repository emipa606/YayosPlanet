using System.Collections.Generic;
using HugsLib;
using HugsLib.Settings;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class modBase : ModBase
{
    public static int tickOfYear = 0;
    public static int eventStart = 1;
    public static int eventCycle = 2;
    public static bool bl_randomType;
    private static bool bl_yyCold = true;
    public static float val_yyCold;
    private static bool bl_yyHot = true;
    public static float val_yyHot;
    private static bool bl_yyTornado = true;
    public static float val_yyTornado = 1f;
    public static float val_threrat = 0.5f;


    // -----------------------------------------


    public static List<GameConditionDef> ar_gc;


    private SettingHandle<bool> bl_randomType_s;

    private SettingHandle<bool> bl_yyCold_s;


    private SettingHandle<bool> bl_yyHot_s;


    private SettingHandle<bool> bl_yyTornado_s;

    private SettingHandle<int> eventCycle_s;

    private SettingHandle<int> eventStart_s;


    private SettingHandle<float> val_threat_s;

    private SettingHandle<float> val_yyCold_s;

    private SettingHandle<float> val_yyHot_s;

    private SettingHandle<float> val_yyTornado_s;
    public override string ModIdentifier => "yayoPlanet";
    private GameConditionDef gc_cold => GameConditionDef.Named("yyCold");
    private GameConditionDef gc_hot => GameConditionDef.Named("yyHot");
    private GameConditionDef gc_Tornado => GameConditionDef.Named("yyTornado");


    // -----------------------------------------

    public override void DefsLoaded()
    {
        eventStart_s = Settings.GetHandle("eventStart", "eventStart_t".Translate(), "eventStart_d".Translate(), 1);

        eventCycle_s = Settings.GetHandle("eventCycle", "eventCycle_t".Translate(), "eventCycle_d".Translate(), 2);


        bl_randomType_s = Settings.GetHandle("bl_randomType", "bl_randomType_t".Translate(),
            "bl_randomType_d".Translate(), false);


        bl_yyCold_s = Settings.GetHandle("bl_yyCold", "bl_yyCold_t".Translate(), "bl_yyCold_d".Translate(), true);

        val_yyCold_s = Settings.GetHandle("val_yyCold", "val_yyCold_t".Translate(), "val_yyCold_d".Translate(), -270f);


        bl_yyHot_s = Settings.GetHandle("bl_yyHot", "bl_yyHot_t".Translate(), "bl_yyHot_d".Translate(), true);

        val_yyHot_s = Settings.GetHandle("val_yyHot", "val_yyHot_t".Translate(), "val_yyHot_d".Translate(), 200f);


        bl_yyTornado_s = Settings.GetHandle("bl_yyTornado", "bl_yyTornado_t".Translate(), "bl_yyTornado_d".Translate(),
            true);

        val_yyTornado_s = Settings.GetHandle("val_yyTornado", "val_yyTornado_t".Translate(),
            "val_yyTornado_d".Translate(), 1f);


        val_threat_s =
            Settings.GetHandle("val_threrat", "val_threrat_t".Translate(), "val_threrat_d".Translate(), 0.5f);

        SettingsChanged();
    }

    public override void SettingsChanged()
    {
        ar_gc = [];

        eventStart = eventStart_s.Value;
        eventCycle = Mathf.Clamp(eventCycle_s.Value, 1, 10);


        bl_randomType = bl_randomType_s.Value;


        bl_yyCold = bl_yyCold_s.Value;
        if (bl_yyCold)
        {
            ar_gc.Add(gc_cold);
        }

        val_yyCold_s.Value = Mathf.Clamp(val_yyCold_s.Value, -500f, -1f);
        val_yyCold = val_yyCold_s.Value;


        bl_yyHot = bl_yyHot_s.Value;
        if (bl_yyHot)
        {
            ar_gc.Add(gc_hot);
        }

        val_yyHot_s.Value = Mathf.Clamp(val_yyHot_s.Value, 1f, 1500f);
        val_yyHot = val_yyHot_s.Value;


        bl_yyTornado = bl_yyTornado_s.Value;
        if (bl_yyTornado)
        {
            ar_gc.Add(gc_Tornado);
        }

        val_yyTornado_s.Value = Mathf.Clamp(val_yyTornado_s.Value, 0.2f, 3f);
        val_yyTornado = val_yyTornado_s.Value;


        val_threat_s.Value = Mathf.Clamp(val_threat_s.Value, 0f, 0.9f);
        val_threrat = val_threat_s.Value;
    }
}