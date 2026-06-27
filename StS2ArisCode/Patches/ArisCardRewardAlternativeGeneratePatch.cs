using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rewards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(CardRewardAlternative), nameof(CardRewardAlternative.Generate))]
public static class ArisCardRewardAlternativeGeneratePatch
{
    public static bool Prefix(CardReward cardReward, ref IReadOnlyList<CardRewardAlternative> __result)
    {
        List<CardRewardAlternative> alternatives = [];
        if (cardReward.CanSkip)
        {
            alternatives.Add(new CardRewardAlternative("Skip", PostAlternateCardRewardAction.EndSelectionAndDoNotCompleteReward));
        }

        if (cardReward.CanReroll)
        {
            alternatives.Add(new CardRewardAlternative("REROLL", () =>
            {
                cardReward.Reroll();
                return Task.CompletedTask;
            }, PostAlternateCardRewardAction.DoNothing));
        }

        Hook.ModifyCardRewardAlternatives(cardReward.Player.RunState, cardReward.Player, cardReward, alternatives);
        __result = alternatives;
        return false;
    }
}
