# GitHub Copilot Instructions for Yayo's Planet (Continued)

## Mod Overview and Purpose

**Mod Name:** Yayo's Planet (Continued)  
**Author:** YAYO, Horsefry  
**PackageId:** Mlie.YayosPlanet  
**Parser:** tree-sitter / ok  
**Counts:** 12 C# files, 8 XML files, 12 types, 68 members, 4 XML defs  
**Dependencies:** brrainz.harmony, UnlimitedHugs.HugsLib

**Description:**
Yayo's Planet (Continued) is an update of the original YAYO mod, Horsefry's mod. It introduces seasonal catastrophes every fourth quarter, such as Ice Age, Inferno, and Sandstorm, to enhance the survival challenge of RimWorld. The mod allows players to prepare for these disasters and adjust the temperature in the game's options.

**Key Features and Systems:**

- **Seasonal Catastrophes:** Players experience Ice Age (very cold), Inferno (very hot), and Sandstorm (with tornadoes) every fourth quarter of the game year.
- **Temperature Control:** Players can adjust the temperature in the game's options and use various items to manage temperature.
- **Mod Integration:** The mod is designed to work with other mods like Dubs Bad Hygiene for temperature-related items and Yayo's Nature for changing biomes.

## Coding Patterns and Conventions

- **C# Files:**
  - **Core.cs:** Contains the main logic for the mod, including map component ticks.
  - **WeatherOverlay_fastWind.cs:** Handles weather overlays for fast wind effects.
  - **YayoPlanetMod.cs:** Contains the main mod class with settings management and initialization.
  - **YayoPlanetSettings.cs:** Manages the mod's settings and data exposure.
  - **yyTornado.cs:** Defines the tornado entity with various behaviors and properties.

- **XML Files:**
  - **GameCondition_yayoPlanet.xml:** Defines the game conditions for the seasonal catastrophes.
  - **tornado.xml:** Defines the tornado entity with its properties and behavior.
  - **UpdateInfo.xml:** Contains update information for the mod.
  - **About.xml:** Provides information about the mod and its dependencies.

- **Coding Standards:**
  - Use PascalCase for class and method names.
  - Use camelCase for variable names.
  - Follow the RimWorld coding standards and conventions.

## XML Integration

- **GameConditionDefs:** Define game conditions for the seasonal catastrophes.
  xml
  <GameConditionDef>
    <defName>yayoPlanet_IceAge</defName>
    <label>Ice Age</label>
    <description>A very cold period.</description>
    <weather>
      <weatherDef>Weather_Snowy</weatherDef>
    </weather>
  </GameConditionDef>
  

- **ThingDefs:** Define new things, such as tornadoes.
  xml
  <ThingDef>
    <defName>Tornado</defName>
    <label>Tornado</label>
    <description>A destructive tornado.</description>
    <category>Weather</category>
    <thingClass>ThingWithComps</thingClass>
    <comps>
      <li Class="CompProperties_Damage">
        <damageDef>DamageDef_Tornado</damageDef>
        <amount>10</amount>
      </li>
    </comps>
  </ThingDef>
  

- **UpdateInfo:** Define update information for the mod.
  xml
  <TabulaRasa.UpdateDef>
    <modName>Yayo's Planet (Continued)</modName>
    <version>1.7.0</version>
    <notes>
      <li>Added new disaster types.</li>
      <li>Improved temperature control.</li>
    </notes>
  </TabulaRasa.UpdateDef>
  

## Harmony Patching

Yayo's Planet (Continued) uses Harmony for patching existing RimWorld code to implement its features. Here are some examples of Harmony patches:

- **Patch for MapComponentTick:**
  csharp
  [HarmonyPatch(typeof(MapComponent), nameof(MapComponent.MapComponentTick))]
  public static class MapComponentTickPatch
  {
      public static void Postfix(MapComponent __instance)
      {
          if (__instance.Map.GameConditionManager.ConditionIsActive(GameConditionDefOf.IceAge))
          {
              // Custom logic for Ice Age
          }
      }
  }
  

- **Patch for WeatherOverlay_fastWind:**
  csharp
  [HarmonyPatch(typeof(WeatherOverlay_fastWind), nameof(WeatherOverlay_fastWind.Draw))]
  public static class WeatherOverlay_fastWindPatch
  {
      public static void Postfix(WeatherOverlay_fastWind __instance)
      {
          if (__instance.Map.GameConditionManager.ConditionIsActive(GameConditionDefOf.Inferno))
          {
              // Custom logic for Inferno weather
          }
      }
  }
  

## Suggestions for Copilot

1. **Explore Existing Patches:** Use existing Harmony patches in the mod to understand how to patch RimWorld code.
2. **Modify XML Defs:** Experiment with modifying existing XML defs to add new features or tweak existing ones.
3. **Implement New Features:** Suggest new features for the mod, such as additional disaster types or improved temperature control systems.
4. **Fix Bugs:** Identify and fix any bugs or issues in the mod's code.
5. **Optimize Performance:** Optimize the mod's performance by refining code and reducing unnecessary computations.

## Contributing

- **Source Code:** The source code is included with the mod.
- **Discord Support:** Join the RimWorld Discord for support and collaboration.
- **Error Reporting:** Use the Log Uploader to share logs when reporting errors.
- **Mod Sorting:** Use RimSort to sort mods for better performance.

Feel free to modify and enhance Yayo's Planet (Continued) with your own ideas. If you have any questions or suggestions, please contact the author on the RimWorld Discord.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

