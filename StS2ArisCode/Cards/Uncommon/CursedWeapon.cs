using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class CursedWeapon() : StS2ArisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private bool _wasInHandBeforeOtherCardPlayed;
    private bool _movedDuringOtherCardPlay;

    protected override bool ShouldGlowRedInternal => Pile?.Type == PileType.Hand;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new HpLossVar(2),
        new DynamicVar("Increase", 4m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play).Targeting(play.Target).Execute(choiceContext);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _wasInHandBeforeOtherCardPlayed = cardPlay.Card != this
            && cardPlay.Card.Owner == Owner
            && Pile?.Type == PileType.Hand;
        _movedDuringOtherCardPlay = false;
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (_wasInHandBeforeOtherCardPlayed && card == this && oldPileType == PileType.Hand)
        {
            _movedDuringOtherCardPlay = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_wasInHandBeforeOtherCardPlayed ||
            _movedDuringOtherCardPlay ||
            Pile?.Type != PileType.Hand ||
            cardPlay.Card.Owner != Owner)
        {
            ResetTriggerState();
            return;
        }

        await CreatureCmdCompat.DamageFromCard(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unpowered | ValueProp.Move, Owner.Creature, this, cardPlay);
        DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
        ResetTriggerState();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }

    private void ResetTriggerState()
    {
        _wasInHandBeforeOtherCardPlayed = false;
        _movedDuringOtherCardPlay = false;
    }
}
