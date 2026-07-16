using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class BurningTime() : StS2ArisCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self), IArisOutputCard, IOverload
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [ArisKeywords.Output, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Output),
        HoverTipFactory.FromKeyword(ArisKeywords.Overload)
    ];

    public Task OnOverload(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.ToList())
        {
            card.SetToFreeThisTurn();
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
