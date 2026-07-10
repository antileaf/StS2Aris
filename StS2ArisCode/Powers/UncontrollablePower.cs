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
        if (power == this && amount > 0)
        {
            ApplyReplayToExistingShocks((int)amount);
        }

        return Task.CompletedTask;
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator?.Creature == Owner && card is Shock)
        {
            card.BaseReplayCount += Amount;
            Flash();
        }

        return Task.CompletedTask;
    }

    private void ApplyReplayToExistingShocks(int amount)
    {
        var player = Owner.Player;
        if (player?.PlayerCombatState == null || amount <= 0)
        {
            return;
        }

        foreach (var shock in player.PlayerCombatState.AllCards.OfType<Shock>())
        {
            shock.BaseReplayCount += amount;
        }
    }
}
