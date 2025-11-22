using HarmonyLib;
using Mlie;
using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using Verse;

namespace yayoPlanet;

public class YayoPlanetMod : Mod
{
    private static string currentVersion;
    public static List<GameConditionDef> ar_gc = [];
    public static bool bl_randomType;
    public static bool bl_yyCold = true;
    public static bool bl_yyHot = true;
    public static bool bl_yyTornado = true;
    public static int eventCycle = 2;
    public static int eventStart = 1;

    public static YayoPlanetMod Instance;
    public static YayoPlanetSettings Settings;

    public static int tickOfYear = 0;
    public static float val_threat = 0.5f;
    public static float val_yyCold;
    public static float val_yyHot;
    public static float val_yyTornado = 1f;

    private string valColdBuffer;
    private string valHotBuffer;
    private string valThreatBuffer;
    private string valTornadoBuffer;

    public YayoPlanetMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<YayoPlanetSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);

        valColdBuffer = Settings.val_yyCold.ToString();
        valHotBuffer = Settings.val_yyHot.ToString();
        valTornadoBuffer = Settings.val_yyTornado.ToString();
        valThreatBuffer = Settings.val_threat.ToString();

        Instance = this;

        importOldHugsLibSettings();

        ApplySettings();

        new Harmony("yayoPlanet").PatchAll(Assembly.GetExecutingAssembly());
    }

    // --- Equivalent to HugsLib SettingsChanged() ---
    private void ApplySettings()
    {
        ar_gc = [];

        var gc_cold = DefDatabase<GameConditionDef>.GetNamed("yyCold", false);
        var gc_hot = DefDatabase<GameConditionDef>.GetNamed("yyHot", false);
        var gc_Tornado = DefDatabase<GameConditionDef>.GetNamed("yyTornado", false);

        eventStart = Settings.eventStart;
        eventCycle = Mathf.Clamp(Settings.eventCycle, 1, 10);

        bl_randomType = Settings.bl_randomType;

        bl_yyCold = Settings.bl_yyCold;
        if(bl_yyCold)
        {
            ar_gc.Add(gc_cold);
        }

        Settings.val_yyCold = Mathf.Clamp(Settings.val_yyCold, -500f, -1f);
        val_yyCold = Settings.val_yyCold;

        bl_yyHot = Settings.bl_yyHot;
        if(bl_yyHot)
        {
            ar_gc.Add(gc_hot);
        }

        Settings.val_yyHot = Mathf.Clamp(Settings.val_yyHot, 1f, 1500f);
        val_yyHot = Settings.val_yyHot;

        bl_yyTornado = Settings.bl_yyTornado;
        if(bl_yyTornado)
        {
            ar_gc.Add(gc_Tornado);
        }

        Settings.val_yyTornado = Mathf.Clamp(Settings.val_yyTornado, 0.2f, 3f);
        val_yyTornado = Settings.val_yyTornado;

        Settings.val_threat = Mathf.Clamp(Settings.val_threat, 0f, 0.9f);
        val_threat = Settings.val_threat;
    }


    private static void importOldHugsLibSettings()
    {
        var hugsLibConfig = Path.Combine(GenFilePaths.SaveDataFolderPath, "HugsLib", "ModSettings.xml");
        if(!new FileInfo(hugsLibConfig).Exists)
        {
            return;
        }

        var xml = XDocument.Load(hugsLibConfig);
        var modNodeName = "yayoPlanet";

        var modSettings = xml.Root?.Element(modNodeName);
        if(modSettings == null)
        {
            return;
        }

        foreach(var modSetting in modSettings.Elements())
        {
            if(modSetting.Name == "eventStart_s")
            {
                Settings.eventStart = int.Parse(modSetting.Value);
            }
            if(modSetting.Name == "eventCycle_s")
            {
                Settings.eventCycle = int.Parse(modSetting.Value);
            }
            if(modSetting.Name == "bl_randomType_s")
            {
                Settings.bl_randomType = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "bl_yyCold_s")
            {
                Settings.bl_yyCold = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "val_yyCold_s")
            {
                Settings.val_yyCold = float.Parse(modSetting.Value);
            }
            if(modSetting.Name == "bl_yyHot_s")
            {
                Settings.bl_yyHot = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "val_yyHot_s")
            {
                Settings.val_yyHot = float.Parse(modSetting.Value);
            }
            if(modSetting.Name == "bl_yyTornado_s")
            {
                Settings.bl_yyTornado = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "val_yyTornado_s")
            {
                Settings.val_yyTornado = int.Parse(modSetting.Value);
            }
            if(modSetting.Name == "val_threat_s")
            {
                Settings.val_threat = int.Parse(modSetting.Value);
            }
        }

        Instance.ApplySettings();
        xml.Root.Element(modNodeName)?.Remove();
        xml.Save(hugsLibConfig);
        Log.Message($"[{modNodeName}]: Imported old HugLib-settings");
    }


    // --- UI ---
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing = new Listing_Standard();
        listing.Begin(rect);

        // -------- eventStart (int entry) --------
        listing.IntEntryWithAdjuster("eventStart_t", ref Settings.eventStart, 1, 1, 99999);

        // -------- eventCycle (int entry) --------        
        listing.IntEntryWithAdjuster("eventCycle_t", ref Settings.eventCycle, 1, 1, 10);

        // -------- random type checkbox --------
        listing.CheckboxLabeled("bl_randomType_t".Translate(), ref Settings.bl_randomType);

        // -------- yyCold checkbox --------
        listing.CheckboxLabeled("bl_yyCold_t".Translate(), ref Settings.bl_yyCold);

        // -------- val_yyCold slider --------
        {
            Rect row = listing.GetRect(Text.LineHeight);
            Rect left = row.LeftPart(0.45f).Rounded();
            Rect right = row.RightPart(0.55f).Rounded();
            Widgets.Label(left, "val_yyCold_t".Translate(Settings.val_yyCold));
            Settings.val_yyCold = Widgets.HorizontalSlider(right, Settings.val_yyCold, -500f, -1f);
            valColdBuffer = Settings.val_yyCold.ToString();
        }

        // -------- yyHot checkbox --------
        listing.CheckboxLabeled("bl_yyHot_t".Translate(), ref Settings.bl_yyHot);

        // -------- val_yyHot slider --------
        {
            Rect row = listing.GetRect(Text.LineHeight);
            Rect left = row.LeftPart(0.45f).Rounded();
            Rect right = row.RightPart(0.55f).Rounded();
            Widgets.Label(left, "val_yyHot_t".Translate(Settings.val_yyHot));
            Settings.val_yyHot = Widgets.HorizontalSlider(right, Settings.val_yyHot, 1f, 1500f);
            valHotBuffer = Settings.val_yyHot.ToString();
        }

        // -------- yyTornado checkbox --------
        listing.CheckboxLabeled("bl_yyTornado_t".Translate(), ref Settings.bl_yyTornado);

        // -------- val_yyTornado slider --------
        {
            Rect row = listing.GetRect(Text.LineHeight);
            Rect left = row.LeftPart(0.45f).Rounded();
            Rect right = row.RightPart(0.55f).Rounded();
            Widgets.Label(left, "val_yyTornado_t".Translate(Settings.val_yyTornado));
            Settings.val_yyTornado = Widgets.HorizontalSlider(right, Settings.val_yyTornado, 0.2f, 3f);
            valTornadoBuffer = Settings.val_yyTornado.ToString();
        }

        // -------- val_threat slider (with tooltip) --------
        {
            Rect row = listing.GetRect(Text.LineHeight);
            Rect left = row.LeftPart(0.45f).Rounded();
            Rect right = row.RightPart(0.55f).Rounded();
            Widgets.Label(left, "val_threat_t".Translate(Settings.val_threat));
            if(Mouse.IsOver(row))
            {
                TooltipHandler.TipRegion(row, "val_threat_d".Translate());
            }

            Settings.val_threat = Widgets.HorizontalSlider(right, Settings.val_threat, 0f, 0.9f);
            valThreatBuffer = Settings.val_threat.ToString();
        }

        if(listing.ButtonText("YP_Reset".Translate()))
        {
            reset();
        }

        if(currentVersion != null)
        {
            listing.Gap();
            GUI.contentColor = Color.gray;
            listing.Label("YP_CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }
        listing.End();
    }


    public override string SettingsCategory() { return "YP_ModName".Translate(); }

    public override void WriteSettings()
    {
        base.WriteSettings();
        ApplySettings();
    }

    private void reset()
    {
        Settings.eventStart = 1;
        Settings.eventCycle = 2;
        Settings.bl_randomType = false;
        Settings.bl_yyCold = true;
        Settings.val_yyCold = -270f;
        Settings.bl_yyHot = true;
        Settings.val_yyHot = 200f;
        Settings.bl_yyTornado = true;
        Settings.val_yyTornado = 1f;
        Settings.val_threat = 0.5f;
        valColdBuffer = Settings.val_yyCold.ToString();
        valHotBuffer = Settings.val_yyHot.ToString();
        valTornadoBuffer = Settings.val_yyTornado.ToString();
        valThreatBuffer = Settings.val_threat.ToString();
        ApplySettings();
    }
}
