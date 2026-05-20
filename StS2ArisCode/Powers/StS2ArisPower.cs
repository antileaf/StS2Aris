using BaseLib.Abstracts;
using BaseLib.Extensions;
using StS2Aris.StS2ArisCode.Extensions;
using Godot;

namespace StS2Aris.StS2ArisCode.Powers;

public abstract class StS2ArisPower : CustomPowerModel
{
    //Loads from StS2Aris/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}