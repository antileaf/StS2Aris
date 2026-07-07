using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class GameArtPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner)
        {
            return Task.CompletedTask;
        }

        if (ApplyToGeneratedCard(card, Amount))
        {
            Flash();
        }

        return Task.CompletedTask;
    }

    public static bool ApplyToGeneratedCard(CardModel card, decimal amount)
    {
        if (card.Enchantment != null)
        {
            return false;
        }

        if (card.Type == CardType.Attack && ModelDb.Enchantment<Inky>().CanEnchant(card))
        {
            CardCmd.Enchant<Inky>(card, amount);
        }
        else if (card.Type == CardType.Skill && ModelDb.Enchantment<Swift>().CanEnchant(card))
        {
            CardCmd.Enchant<Swift>(card, amount);
        }
        else
        {
            return false;
        }

        if (card is GameScenario scenario)
        {
            scenario.RefreshGeneratedValues();
        }

        return true;
    }
}
