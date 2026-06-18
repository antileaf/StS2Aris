using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Utils;

public static class ArisHoverTips
{
    public static IHoverTip ChargePower()
    {
        var power = ModelDb.Power<ChargePower>();
        var description = power.Description;
        power.DynamicVars.AddTo(description);
        return new HoverTip(power, description.GetFormattedText(), isSmart: false);
    }
}
