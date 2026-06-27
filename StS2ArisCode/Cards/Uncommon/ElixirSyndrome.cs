using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class ElixirSyndrome() : ArisQuestCard<Elixir>(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override int QuestGoal => 3;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromCard<Elixir>(IsUpgraded)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Task.CompletedTask;
    }

    public override async Task AfterPotionDiscarded(PotionModel potion)
    {
        if (potion.Owner == Owner)
        {
            await AdvanceQuest();
        }
    }

}



