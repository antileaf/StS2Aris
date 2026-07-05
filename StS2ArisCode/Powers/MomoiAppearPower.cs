using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class MomoiAppearPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.Player == null)
        {
            return;
        }

        var player = Owner.Player;
        var scenario = MomoiScenarioGenerator.Create(player, combatState, Math.Max(0, (int)Amount - 1));
        var refuse = combatState.CreateCard<ChooseRefuse>(player);

        CardModel[] choices = [scenario, refuse];
        var selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            choices,
            player);

        if (selected is GameScenario selectedScenario)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selectedScenario, PileType.Hand, player);
            return;
        }

        await PowerCmd.ModifyAmount(choiceContext, this, 1m, Owner, null);
    }
}
