using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Patches;

[HarmonyPatch(typeof(Inky), nameof(Inky.OnPlay))]
public static class InkyRandomTargetPatch
{
    public static bool Prefix(Inky __instance, PlayerChoiceContext choiceContext, CardPlay? cardPlay, ref Task __result)
    {
        var card = __instance.Card;
        if (card.Type != CardType.Attack || card.TargetType == TargetType.AllEnemies || cardPlay?.Target != null)
        {
            return true;
        }

        if (card.TargetType != TargetType.RandomEnemy)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = card is GameScenario or Shock or HeroSword
            ? Task.CompletedTask
            : ApplyWeakToRandomEnemy(__instance, choiceContext);
        return false;
    }

    private static async Task ApplyWeakToRandomEnemy(Inky inky, PlayerChoiceContext choiceContext)
    {
        var card = inky.Card;
        var combatState = card.CombatState;
        if (combatState == null)
        {
            return;
        }

        var target = card.Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
        if (target != null)
        {
            await PowerCmd.Apply<WeakPower>(
                choiceContext,
                target,
                inky.DynamicVars.Weak.BaseValue,
                card.Owner.Creature,
                card);
        }
    }
}
