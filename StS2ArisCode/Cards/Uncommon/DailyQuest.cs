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
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class DailyQuest() : ArisQuestCard<DailyQuest>(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int QuestGoal => 3;

    protected override bool RewardInheritsUpgrade => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromCard<DailyQuest>(false)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new DynamicVar("Magic", 10m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play).WithHitFx("vfx/vfx_attack_blunt").Targeting(play.Target).Execute(choiceContext);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        await AdvanceQuest();
    }

    protected override async Task BeforeQuestComplete()
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Magic"].BaseValue);
        if (IsUpgraded)
        {
            await PlayerCmd.GainGold(50m, Owner);
        }
    }

    public override async Task<CardPileAddResult?> ApplyReplicaReward(bool forceUpgrade)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Magic"].BaseValue);
        if (forceUpgrade || IsUpgraded)
        {
            await PlayerCmd.GainGold(50m, Owner);
        }

        return await CardPileCmd.Add(CreateRewardCard(), PileType.Deck);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(9m);
    }
}


