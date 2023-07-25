using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_fastWind : SkyOverlay
{
    private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld");

    public WeatherOverlay_fastWind()
    {
        worldOverlayMat = FogOverlayWorld;
        worldOverlayPanSpeed1 = 0.05f;
        worldOverlayPanSpeed2 = 0.04f;
        worldPanDir1 = new Vector2(1f, 1f);
        worldPanDir2 = new Vector2(1f, -1f);
    }
}