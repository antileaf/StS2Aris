using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class AutoHuntPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.Player == null || Owner.IsDead)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            var levelUp = combatState.CreateCard<LevelUp>(Owner.Player);
            await CardCmd.AutoPlay(choiceContext, levelUp, null);
        }
    }
}
