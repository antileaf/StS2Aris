using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Relics;

public class Battery : StS2ArisRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ShockAmount", 3m)
    ];

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        Creature? owner = Owner?.Creature;
        if (owner == null || owner != applier || target.Side == owner.Side || canonicalPower is not ShockPower
            || amount <= 0 || target.GetPower<ShockPower>() != null)
        {
            return false;
        }

        Flash();
        modifiedAmount = amount + DynamicVars["ShockAmount"].BaseValue;
        return true;
    }
}
