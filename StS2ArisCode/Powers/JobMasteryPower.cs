using MegaCrit.Sts2.Core.Entities.Powers;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobMasteryPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
