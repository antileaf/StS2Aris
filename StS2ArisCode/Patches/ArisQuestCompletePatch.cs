using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using StS2Aris.StS2ArisCode.Mechanics;
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
            if (CombatManager.Instance.IsInProgress && player.PlayerCombatState != null)
            {
                eggCount += player.PlayerCombatState.AllCards.Count(static card => card is ByrdonisEgg);
            }

            ArisQuestProgress.MarkCompleted(player, eggCount);
        }
    }
}
