using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobWizardPower : ArisJobPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 3m)];
    public override string AnimationSuffix => "Wizard";

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var target = power.Owner;
        if (applier != Owner || target.Side == Owner.Side || power.Type != PowerType.Debuff)
        {
            return;
        }

        Flash();
        var repairAmount = EquipmentCard?.DynamicVars["Magic"].IntValue ?? DynamicVars["Amount"].IntValue;
        await PowerCmd.Apply<EndTurnBlockPower>(choiceContext, Owner, repairAmount + LevelBonus, Owner, EquipmentCard);
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext)
    {
        if (EquipmentCard == null)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(Owner, EquipmentCard.DynamicVars.Block.BaseValue, ValueProp.Move, null);
    }
}
