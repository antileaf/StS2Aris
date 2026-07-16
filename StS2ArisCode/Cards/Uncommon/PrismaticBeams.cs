using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class PrismaticBeams() : StS2ArisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IOverload
{
    private decimal _temporaryDamageBonus;
    private bool _returnToHandAfterPlay;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Overload)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new DynamicVar("Magic", 3m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        _returnToHandAfterPlay = false;
        if (play.Target == null)
            return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play).Targeting(play.Target).Execute(choiceContext);
    }

    public Task OnOverload(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increase = DynamicVars["Magic"].BaseValue;
        DynamicVars.Damage.BaseValue += increase;
        _temporaryDamageBonus += increase;
        _returnToHandAfterPlay = true;
        Owner.PlayerCombatState?.RecalculateCardValues();
        return Task.CompletedTask;
    }

    protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
    {
        var (pileType, position) = base.GetResultPileTypeAndPositionForCardPlay();
        return _returnToHandAfterPlay && pileType == PileType.Discard
            ? (PileType.Hand, CardPilePosition.Bottom)
            : (pileType, position);
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(Owner.Creature) && _temporaryDamageBonus != 0m)
        {
            DynamicVars.Damage.BaseValue -= _temporaryDamageBonus;
            _temporaryDamageBonus = 0m;
            Owner.PlayerCombatState?.RecalculateCardValues();
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Magic"].UpgradeValueBy(1m);
    }
}


