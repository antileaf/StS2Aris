using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(TokenCardPool))]
public class NeatCompression() : StS2ArisCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownDeckVersion = DeckVersion;
        if (ownDeckVersion?.Pile?.Type == PileType.Deck)
        {
            await CardPileCmd.RemoveFromDeck(ownDeckVersion);
            DeckVersion = null;
        }

        if (!IsUpgraded)
        {
            return;
        }

        var upgradeTarget = PileType.Deck.GetPile(Owner).Cards
            .Where(static card => card.IsUpgradable)
            .ToList()
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection)
            .FirstOrDefault();

        if (upgradeTarget != null)
        {
            CardCmd.Upgrade(upgradeTarget);
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (!HasBeenRemovedFromState && Owner != null)
        {
            room.AddExtraReward(Owner, new MegaCrit.Sts2.Core.Rewards.CardRemovalReward(Owner));
        }

        return Task.CompletedTask;
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
}



