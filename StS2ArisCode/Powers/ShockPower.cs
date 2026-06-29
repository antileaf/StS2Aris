using MegaCrit.Sts2.Core.Commands;
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

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || !props.IsPoweredAttack() || Owner.IsDead)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        Flash();
        var targets = Owner.GetPower<TransformerPower>() == null
            ? [Owner]
            : combatState.GetCreaturesOnSide(Owner.Side).Where(static creature => creature.IsAlive).ToList();

        await CreatureCmd.Damage(choiceContext, targets, Amount, DamageProps.nonCardHpLoss, Applier, null);
        await PowerCmd.Decrement(this);
    }
}
