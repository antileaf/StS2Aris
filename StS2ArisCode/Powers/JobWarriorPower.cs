using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobWarriorPower : ArisJobPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 3m)];
    public override string AnimationSuffix => "Warrior";

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.TotalDamage <= 0 || Owner.IsDead)
        {
            return;
        }

        Flash();
        var amount = EquipmentCard?.DynamicVars["Magic"].IntValue ?? DynamicVars["Amount"].IntValue;
        await PowerCmd.Apply<EndTurnBlockPower>(choiceContext, Owner, amount + LevelBonus, Owner, EquipmentCard);
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (EquipmentCard == null)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(Owner, EquipmentCard.DynamicVars.Block.BaseValue, ValueProp.Move, null);
    }
}
