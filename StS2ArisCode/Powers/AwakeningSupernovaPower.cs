using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class AwakeningSupernovaPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not ChargePower || power.Owner != Owner || amount <= 0 || Owner.IsDead)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<AwakeningSupernovaStrengthPower>(choiceContext, Owner, amount * Amount, Owner, cardSource);
    }
}
