using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
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
public class CreatedApotheosis() : StS2ArisCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Magic", 4m)
    ];

    protected override Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var cards = new[] { PileType.Draw, PileType.Hand, PileType.Discard }
            .SelectMany(pileType => pileType.GetPile(Owner).Cards)
            .Where(card => card != this && card.IsUpgradable)
            .TakeRandom(DynamicVars["Magic"].IntValue, Owner.RunState.Rng.CombatCardSelection)
            .ToList();

        foreach (var card in cards)
        {
            CardCmd.Upgrade(card);
            CardCmd.Preview(card);
        }

        return Task.CompletedTask;
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Innate];

    protected override void OnUpgrade()
    {
    }
}




