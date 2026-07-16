using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class ChargeRetentionPower : StS2ArisPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ChargePower>(1m)];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
