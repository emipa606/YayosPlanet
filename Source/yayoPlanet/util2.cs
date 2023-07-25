using UnityEngine;
using Verse;

namespace yayoPlanet;

[StaticConstructorOnStartup]
public static class util2
{
    public static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado",
        ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
}