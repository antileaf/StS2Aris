using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using StS2Aris.StS2ArisCode.Mechanics;
using ArisBingoBoard = StS2Aris.StS2ArisCode.Cards.BingoBoard;
using ArisCharacter = StS2Aris.StS2ArisCode.Character.StS2Aris;

namespace StS2Aris.StS2ArisCode.Patches;

public static class ArisQuestCompletePatch
{
    [HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.CompleteQuest))]
    public static class PlayerCmdCompleteQuestPatch
    {
        public static void Postfix(CardModel questCard)
        {
            ArisQuestProgress.MarkCompleted(questCard);

            if (questCard.Owner.Character is ArisCharacter && questCard.Type == CardType.Quest)
            {
                TaskHelper.RunSafely(ArisBingoBoard.AdvanceBoardsForCompletedQuest(questCard.Owner, questCard));
            }
        }
    }

    [HarmonyPatch(typeof(Byrdpip), nameof(Byrdpip.AfterObtained))]
    public static class ByrdpipAfterObtainedPatch
    {
        public static void Prefix(Byrdpip __instance)
        {
            var player = __instance.Owner;
            if (player.Character is not ArisCharacter)
            {
                return;
            }

            var eggCount = PileType.Deck.GetPile(player).Cards.Count(static card => card is ByrdonisEgg);
            var eggCard = PileType.Deck.GetPile(player).Cards.FirstOrDefault(static card => card is ByrdonisEgg);
            if (CombatManager.Instance.IsInProgress && player.PlayerCombatState != null)
            {
                eggCount += player.PlayerCombatState.AllCards.Count(static card => card is ByrdonisEgg);
                eggCard ??= player.PlayerCombatState.AllCards.FirstOrDefault(static card => card is ByrdonisEgg);
            }

            if (eggCard != null)
            {
                ArisQuestProgress.MarkCompletedQuestType(player, eggCard.Id.Entry);
                TaskHelper.RunSafely(ArisBingoBoard.AdvanceBoardsForCompletedQuest(player, eggCard));
            }

            ArisQuestProgress.MarkCompleted(player, eggCount);
        }
    }
}
