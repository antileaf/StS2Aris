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
using MegaCrit.Sts2.Core.Saves.Runs;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class BingoBoard() : ArisQuestCard<BingoBoard>(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const char QuestTypeSeparator = '|';
    private string _completedQuestTypes = "";

    public override int QuestGoal => 3;

    public override int QuestProgressCurrent => int.Clamp(GetCompletedQuestTypes().Count, 0, QuestProgressGoal);

    public override int QuestProgressGoal => QuestGoal;

    [SavedProperty]
    public string CompletedQuestTypes
    {
        get => _completedQuestTypes;
        set
        {
            AssertMutable();
            _completedQuestTypes = value ?? "";
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromKeyword(ArisKeywords.Quest),
        HoverTipFactory.FromKeyword(ArisKeywords.Reward),
        HoverTipFactory.FromCard<BingoBoard>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(2m),
        new MaxHpVar(7m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars.Dexterity.BaseValue, Owner.Creature, this);
    }

    protected override async Task ApplyQuestReward()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        await CardCmd.Transform(this, CreateRewardCard());
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Dexterity.UpgradeValueBy(1m);
        DynamicVars.MaxHp.UpgradeValueBy(2m);
    }

    private async Task CompleteIfSatisfied()
    {
        await CompleteQuestIf(QuestProgressCurrent >= QuestProgressGoal);
    }

    private bool TryRecordQuest(CardModel questCard)
    {
        if (questCard == this)
        {
            return false;
        }

        var questTypes = GetCompletedQuestTypes();
        if (!questTypes.Add(questCard.Id.Entry))
        {
            return false;
        }

        CompletedQuestTypes = string.Join(QuestTypeSeparator, questTypes.Order(StringComparer.Ordinal));
        return true;
    }

    private HashSet<string> GetCompletedQuestTypes()
    {
        return (CompletedQuestTypes ?? "")
            .Split(QuestTypeSeparator, StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.Ordinal);
    }

    public static async Task AdvanceBoardsForCompletedQuest(Player player, CardModel questCard)
    {
        foreach (var board in PileType.Deck.GetPile(player).Cards.OfType<BingoBoard>().ToList())
        {
            if (board.TryRecordQuest(questCard))
            {
                await board.CompleteIfSatisfied();
            }
        }
    }
}
