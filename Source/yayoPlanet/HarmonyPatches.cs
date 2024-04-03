using System.Reflection;
using HarmonyLib;
using Verse;

namespace yayoPlanet;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("yayoPlanet").PatchAll(Assembly.GetExecutingAssembly());
    }
}