using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace StS2Aris.StS2ArisCode.Cards;

public abstract class ArisQuestCard<TReward>(int cost, CardType type, CardRarity rarity, TargetType target)
    : StS2ArisCard(cost, type, rarity, target), IArisQuestProgressCard where TReward : CardModel
{
    private int _questRemaining;

    public override bool IsArisQuest => true;

    public abstract int QuestGoal { get; }

    public virtual int QuestProgressCurrent => int.Clamp(QuestGoal - QuestRemaining, 0, QuestProgressGoal);

    public virtual int QuestProgressGoal => QuestGoal;

    protected virtual bool RewardInheritsUpgrade => true;

    [SavedProperty]
    public int QuestRemaining
    {
        get => _questRemaining;
        set
        {
            AssertMutable();
            _questRemaining = value;
        }
    }

    public override void AfterCreated()
    {
        base.AfterCreated();
        EnsureQuestInitialized();
    }

    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        EnsureQuestInitialized();
    }

    protected async Task AdvanceQuest(int amount = 1)
    {
        if (!CanAdvanceQuest())
        {
            return;
        }

        EnsureQuestInitialized();
        QuestRemaining = int.Max(0, QuestRemaining - amount);
        if (QuestRemaining == 0)
        {
            await CompleteQuest();
        }
    }

    protected async Task CompleteQuestIf(bool condition)
    {
        if (!condition || !CanAdvanceQuest())
        {
            return;
        }

        await CompleteQuest();
    }

    protected void ResetQuestProgress()
    {
        if (CanAdvanceQuest())
        {
            QuestRemaining = QuestGoal;
        }
    }

    protected virtual Task BeforeQuestComplete()
    {
        return Task.CompletedTask;
    }

    protected virtual TReward CreateRewardCard()
    {
        TReward reward = Owner.RunState.CreateCard<TReward>(Owner);
        if (RewardInheritsUpgrade && IsUpgraded)
        {
            reward.UpgradeInternal();
            reward.FinalizeUpgradeInternal();
        }

        return reward;
    }

    private async Task CompleteQuest()
    {
        if (!CanAdvanceQuest())
        {
            return;
        }

        await BeforeQuestComplete();
        PlayerCmd.CompleteQuest(this);
        await CardCmd.Transform(this, CreateRewardCard());
    }

    private void EnsureQuestInitialized()
    {
        if (QuestRemaining <= 0)
        {
            QuestRemaining = QuestGoal;
        }
    }

    private bool CanAdvanceQuest()
    {
        return Owner != null && Pile?.Type == PileType.Deck && !HasBeenRemovedFromState;
    }

}
