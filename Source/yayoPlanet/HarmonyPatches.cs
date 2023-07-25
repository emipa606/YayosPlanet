using System.Reflection;
using HarmonyLib;
using Verse;

namespace yayoPlanet;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        var harmony = new Harmony("yayoPlanet");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}