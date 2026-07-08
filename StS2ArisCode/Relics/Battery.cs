using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Relics;

public class Battery : StS2ArisRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (Owner?.Creature != applier || target == applier || canonicalPower is not ShockPower || amount <= 0 || target.GetPower<ShockPower>() != null)
        {
            return false;
        }

        Flash();
        modifiedAmount = amount + 1m;
        return true;
    }
}
