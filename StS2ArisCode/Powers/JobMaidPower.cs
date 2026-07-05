using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobMaidPower : ArisJobPower
{
    public override string AnimationSuffix => "Maid";

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }

        var player = PlayerOwner;
        var attackCard = cardPlay?.Card ?? cardSource;
        var hand = player?.PlayerCombatState?.Hand;
        if (player == null || ArisEquipment.GetCurrentJob(player) != this || hand == null ||
            attackCard?.Owner != player || attackCard.Type != CardType.Attack)
        {
            return 1m;
        }

        var attacksInHand = hand.Cards.Count(static card => card.Type == CardType.Attack);
        int effectiveAttackCount;
        if (cardPlay == null)
        {
            if (!hand.Cards.Contains(attackCard))
            {
                return 1m;
            }

            effectiveAttackCount = attacksInHand;
        }
        else
        {
            if (cardPlay.IsAutoPlay || cardPlay.Card != attackCard || attackCard.Pile?.Type != PileType.Play)
            {
                return 1m;
            }

            effectiveAttackCount = attacksInHand + 1;
        }

        return effectiveAttackCount == 1 ? 1m + 0.5m * EffectApplications : 1m;
    }
}
