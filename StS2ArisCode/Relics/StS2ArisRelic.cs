using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Extensions;
using Godot;

namespace StS2Aris.StS2ArisCode.Relics;

[Pool(typeof(StS2ArisRelicPool))]
public abstract class StS2ArisRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}