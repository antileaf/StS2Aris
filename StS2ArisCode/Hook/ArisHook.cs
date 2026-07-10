using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Hooks;

public static class ArisHook
{
    private static async Task Dispatch<T>(PlayerChoiceContext choiceContext, Player player, Func<T, Task> invoke)
        where T : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        foreach (var model in combatState.IterateHookListeners().OfType<T>().ToList())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await invoke(model);
            choiceContext.PopModel(abstractModel);
        }
    }

    public static Task OnClassChanged(PlayerChoiceContext choiceContext, Player player, CardModel? source)
        => Dispatch<IOnClassChanged>(choiceContext, player, model => model.OnClassChanged(choiceContext, player, source));

    public static Task OnJobChanged(PlayerChoiceContext choiceContext, Player player, ArisJobPower? previousJob, ArisJobPower currentJob, CardModel? source)
        => Dispatch<IOnJobChanged>(choiceContext, player, model => model.OnJobChanged(choiceContext, player, previousJob, currentJob, source));
}
