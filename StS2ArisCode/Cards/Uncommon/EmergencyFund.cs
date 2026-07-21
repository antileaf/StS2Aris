using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class EmergencyFund() : ArisQuestCard<EmergencyFund>(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override int QuestGoal => 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9, ValueProp.Move),
        new DynamicVar("Gold", 120m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, play);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType == RoomType.Shop && Owner.RunState.CurrentMapPointHistoryEntry?.MapPointType == MapPointType.Unknown)
        {
            await CompleteQuest();
        }
    }

    protected override async Task ApplyQuestReward()
    {
        await PlayerCmd.GainGold(DynamicVars["Gold"].BaseValue, Owner);
        await CardPileCmd.RemoveFromDeck(this);
    }

    public override async Task<CardPileAddResult?> ApplyReplicaReward(bool forceUpgrade)
    {
        await PlayerCmd.GainGold(GetReplicaRewardValue("Gold", forceUpgrade), Owner);
        return null;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Gold"].UpgradeValueBy(50m);
    }
}
