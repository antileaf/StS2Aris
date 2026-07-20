using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class UncontrollablePower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not UncontrollablePower || power.Owner != Owner || amount <= 0)
        {
            return Task.CompletedTask;
        }

        foreach (var card in Owner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>())
        {
            TryAddReplays(card, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.IsClone)
        {
            return Task.CompletedTask;
        }

        TryAddReplays(card, Amount);
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        foreach (var card in oldOwner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>())
        {
            if (card is Shock shock)
            {
                shock.BaseReplayCount -= Amount;
            }
        }

        return Task.CompletedTask;
    }

    private void TryAddReplays(CardModel card, int amount)
    {
        if (card.Owner == Owner.Player && card is Shock shock)
        {
            shock.BaseReplayCount += amount;
            Flash();
        }
    }
}
