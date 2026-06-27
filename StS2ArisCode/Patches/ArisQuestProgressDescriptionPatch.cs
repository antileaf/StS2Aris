using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForPile), typeof(PileType), typeof(Creature))]
public static class ArisQuestProgressDescriptionPatch
{
    public static void Postfix(CardModel __instance, PileType pileType, ref string __result)
    {
        if (pileType != PileType.Deck || __instance is not IArisQuestProgressCard quest)
        {
            return;
        }

        LocString progress = new("gameplay_ui", "STS2ARIS-QUEST_PROGRESS.description");
        progress.Add("Current", quest.QuestProgressCurrent);
        progress.Add("Goal", quest.QuestProgressGoal);
        __result = string.Join('\n', __result, progress.GetFormattedText());
    }
}
