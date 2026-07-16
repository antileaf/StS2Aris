using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class FriendsWalkingTogetherPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var player = Owner.Player;
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.IsDead || player == null)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            var shot = combatState.CreateCard<LuminousNovaShot>(player);
            await CardPileCmd.AddGeneratedCardToCombat(shot, PileType.Hand, player);
        }
    }
}
