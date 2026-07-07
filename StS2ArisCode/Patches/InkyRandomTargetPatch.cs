using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Enchantments;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(Inky), nameof(Inky.OnPlay))]
public static class InkyRandomTargetPatch
{
    public static bool Prefix(Inky __instance, PlayerChoiceContext choiceContext, CardPlay? cardPlay, ref Task __result)
    {
        var card = __instance.Card;
        if (card is not GameScenario || card.TargetType != TargetType.RandomEnemy || cardPlay?.Target != null)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}
