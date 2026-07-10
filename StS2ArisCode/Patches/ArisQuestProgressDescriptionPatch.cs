using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch]
public static class ArisQuestProgressDescriptionPatch
{
    public static MethodBase TargetMethod()
    {
        var previewType = typeof(CardModel).GetNestedType("DescriptionPreviewType", BindingFlags.NonPublic)
                          ?? throw new MissingMemberException(typeof(CardModel).FullName, "DescriptionPreviewType");
        return AccessTools.DeclaredMethod(typeof(CardModel), "GetDescriptionForPile", [typeof(PileType), previewType, typeof(Creature)])
               ?? throw new MissingMethodException(typeof(CardModel).FullName, "GetDescriptionForPile");
    }

    [HarmonyPriority(Priority.Last)]
    public static void Postfix(CardModel __instance, PileType pileType, ref string __result)
    {
        if (!IsDeckDescription(__instance, pileType) || __instance is not IArisQuestProgressCard quest || quest.QuestProgressGoal <= 1)
        {
            return;
        }

        LocString progress = new("gameplay_ui", "STS2ARIS-QUEST_PROGRESS.description");
        progress.Add("Current", quest.QuestProgressCurrent);
        progress.Add("Goal", quest.QuestProgressGoal);
        __result = string.Join('\n', __result, progress.GetFormattedText());
    }

    private static bool IsDeckDescription(CardModel card, PileType pileType)
    {
        return pileType == PileType.Deck || card.Pile?.Type == PileType.Deck;
    }
}
