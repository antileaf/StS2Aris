using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class HeroQuest() : ArisQuestCard<SwordOfHero>(-2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private int _lastQuestFloor;

    public override int QuestGoal => 12;

    [SavedProperty]
    public int LastQuestFloor
    {
        get => _lastQuestFloor;
        set
        {
            AssertMutable();
            _lastQuestFloor = value;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromCard<SwordOfHero>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, play);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        int floor = Owner.RunState.TotalFloor;
        if (floor <= 0 || floor == LastQuestFloor)
        {
            return;
        }

        LastQuestFloor = floor;
        await AdvanceQuest();
    }
}



