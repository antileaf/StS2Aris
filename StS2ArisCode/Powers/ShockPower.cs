using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class ShockPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (Owner.IsDead || command.Attacker == null || command.Attacker == Owner || !command.DamageProps.IsPoweredAttack())
        {
            return;
        }

        int triggers = command.Results
            .SelectMany(static results => results)
            .Count(result => result.Receiver == Owner);
        if (triggers <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        for (int i = 0; i < triggers && Amount > 0 && !Owner.IsDead; i++)
        {
            Flash();
            var targets = Owner;

            await CreatureCmd.Damage(choiceContext, targets, Amount, DamageProps.nonCardHpLoss, Applier, null, null);
            await PowerCmd.Decrement(this);
        }
    }
}
