using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobMaidPower : ArisJobPower
{
    private const string DamagePercentKey = "DamagePercent";
    private readonly HashSet<CardModel> _boostedCards = [];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(DamagePercentKey, 50m)];

    public override string AnimationSuffix => "Maid";

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (ShouldBoostPlayedCard(cardPlay))
        {
            _boostedCards.Add(cardPlay.Card);
        }

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _boostedCards.Remove(cardPlay.Card);
        return Task.CompletedTask;
    }

    public decimal ModifyDamageMultiplicativeCompat(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
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
        if (cardPlay != null)
        {
            if (cardPlay.IsAutoPlay || cardPlay.Card != attackCard)
            {
                return 1m;
            }

            return attacksInHand + 1 == 1 ? GetDamageMultiplier() : 1m;
        }

        if (_boostedCards.Contains(attackCard))
        {
            return GetDamageMultiplier();
        }

        if (!hand.Cards.Contains(attackCard))
        {
            return 1m;
        }

        return attacksInHand == 1 ? GetDamageMultiplier() : 1m;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this || power is LevelUpPower && power.Owner == Owner)
        {
            RefreshDamagePercent();
        }

        return Task.CompletedTask;
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        RefreshDamagePercent();
        return Task.CompletedTask;
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var player = equipment?.Owner;
        if (equipment == null || player == null)
        {
            return;
        }

        var selection = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Draw.GetPile(player),
            player,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, equipment.DynamicVars.Cards.IntValue))).ToList();

        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);
        foreach (var card in selection)
        {
            var cardScope = card.CardScope;
            if (cardScope == null)
            {
                continue;
            }

            var replacement = cardScope.CreateCard<CleanUp>(player);
            if (equipment.IsUpgraded)
            {
                CardCmd.Upgrade(replacement);
            }

            await CardCmd.Transform(card, replacement);
        }
    }

    private void RefreshDamagePercent()
    {
        if (DynamicVars.TryGetValue(DamagePercentKey, out var damagePercent))
        {
            damagePercent.BaseValue = 50m * EffectApplications;
        }

        InvokeDisplayAmountChanged();
    }

    private bool ShouldBoostPlayedCard(CardPlay cardPlay)
    {
        var player = PlayerOwner;
        var hand = player?.PlayerCombatState?.Hand;
        return player != null
               && ArisEquipment.GetCurrentJob(player) == this
               && hand != null
               && !cardPlay.IsAutoPlay
               && cardPlay.Card.Owner == player
               && cardPlay.Card.Type == CardType.Attack
               && hand.Cards.All(static card => card.Type != CardType.Attack);
    }

    private decimal GetDamageMultiplier()
    {
        return 1m + 0.5m * EffectApplications;
    }
}
