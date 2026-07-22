using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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
public class Grinding() : ArisQuestCard<ExecutionSword>(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    internal const int RequiredGold = 500;

    public override int QuestGoal => 1;

    public override int QuestProgressCurrent => Math.Clamp((int)Owner.Gold, 0, QuestProgressGoal);

    public override int QuestProgressGoal => RequiredGold;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromCard<ExecutionSword>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Magic", 2m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["Magic"].BaseValue, Owner);
    }

    public override async Task AfterGoldGained(Player player)
    {
        await CompleteQuestIf(player == Owner && player.Gold >= RequiredGold);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && Pile?.Type == PileType.Deck && Owner.Gold >= RequiredGold)
        {
            await CompleteQuestIf(true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Magic"].UpgradeValueBy(1m);
    }
}




