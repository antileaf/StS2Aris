using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Mechanics;
using DailyQuest = StS2Aris.StS2ArisCode.Cards.DailyQuest;

namespace StS2Aris.StS2ArisCode.Utils;

public static class ArisQuestUtils
{
    public static int CountCompletedQuests(MegaCrit.Sts2.Core.Entities.Players.Player? player) => ArisQuestProgress.CountCompletedQuests(player);

    public static CardModel? CreateRewardFor(CardModel quest, bool forceUpgrade = false)
    {
        return quest switch
        {
            DailyQuest dailyQuest => CreateReward<DailyQuest>(dailyQuest, false),
            Diet diet => CreateReward<NeatCompression>(diet, forceUpgrade),
            Grinding grinding => CreateReward<ExecutionSword>(grinding, forceUpgrade),
            RaidAddiction raidAddiction => CreateReward<RaidersLeader>(raidAddiction, forceUpgrade),
            ByrdonisEgg byrdonisEgg => CreateReward<ByrdSwoop>(byrdonisEgg, forceUpgrade),
            _ => null
        };
    }

    public static async Task<CardPileAddResult?> ApplyReplicaRewardFor(CardModel quest, bool forceUpgrade = false)
    {
        switch (quest)
        {
            case DailyQuest dailyQuest:
                await CreatureCmd.Heal(dailyQuest.Owner.Creature, dailyQuest.DynamicVars["Magic"].BaseValue);
                if (forceUpgrade || dailyQuest.IsUpgraded)
                {
                    await PlayerCmd.GainGold(50m, dailyQuest.Owner);
                }

                return await CardPileCmd.Add(CreateReward<DailyQuest>(dailyQuest, false), PileType.Deck);
            case LanternKey:
                await RelicCmd.Obtain<HistoryCourse>(quest.Owner);
                return null;
            case SpoilsMap:
                await PlayerCmd.GainGold(600m, quest.Owner);
                return null;
        }

        var reward = CreateRewardFor(quest, forceUpgrade);
        return reward == null
            ? null
            : await CardPileCmd.Add(reward, PileType.Deck);
    }

    public static bool HasSelectableReplicaReward(CardModel quest)
    {
        return quest is DailyQuest or Diet or Grinding or RaidAddiction or LanternKey or SpoilsMap or ByrdonisEgg;
    }

    private static TReward CreateReward<TReward>(CardModel quest, bool forceUpgrade) where TReward : CardModel
    {
        var reward = quest.Owner.RunState.CreateCard<TReward>(quest.Owner);
        if (forceUpgrade || quest.IsUpgraded)
        {
            reward.UpgradeInternal();
            reward.FinalizeUpgradeInternal();
        }

        return reward;
    }
}
