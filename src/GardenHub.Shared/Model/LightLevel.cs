using System.ComponentModel;

namespace GardenHub.Shared.Model;

public enum LightLevel
{
    [Description("Full Sun")]
    FullSun = 1,
    [Description("Parital Shade")]
    PartialShade = 2,
    [Description("Full Shade")]
    FullShade = 4
}