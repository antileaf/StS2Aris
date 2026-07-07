using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using ArisCharacter = StS2Aris.StS2ArisCode.Character.StS2Aris;

namespace StS2Aris.StS2ArisCode.Mechanics;

public static class ArisQuestProgress
{
    private static readonly SavedSpireField<Player, int> CompletedQuestCount = new(() => 0, "ArisCompletedQuestCount");

    public static int CountCompletedQuests(Player? player)
    {
        return player == null ? 0 : CompletedQuestCount.Get(player);
    }

    public static void MarkCompleted(CardModel questCard)
    {
        var player = questCard.Owner;
        if (player?.Character is not ArisCharacter || !IsTrackedQuest(questCard))
        {
            return;
        }

        CompletedQuestCount.Set(player, CompletedQuestCount.Get(player) + 1);
    }

    public static void MarkCompleted(Player player, int amount)
    {
        if (player.Character is not ArisCharacter || amount <= 0)
        {
            return;
        }

        CompletedQuestCount.Set(player, CompletedQuestCount.Get(player) + amount);
    }

    private static bool IsTrackedQuest(CardModel questCard)
    {
        return questCard is StS2ArisCard { IsArisQuest: true } || questCard.Type == CardType.Quest;
    }
}
