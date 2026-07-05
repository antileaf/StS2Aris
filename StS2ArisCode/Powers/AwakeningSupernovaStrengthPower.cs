using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Extensions;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class AwakeningSupernovaStrengthPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<AwakeningSupernova>();

    public string? CustomPackedIconPath => "awakening_supernova_strength_power.png".PowerImagePath();
    public string? CustomBigIconPath => "awakening_supernova_strength_power.png".BigPowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
}
