using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobWarriorPower : ArisJobPower
{
    public override string AnimationSuffix => "Warrior";

    private decimal RecoveryAmount =>
        (EquipmentCard?.DynamicVars.TryGetValue("Magic", out var recovery) == true
            ? recovery.BaseValue
            : 2m) * EffectApplications;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.TotalDamage <= 0 || Owner.IsDead)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<EndTurnBlockPower>(choiceContext, Owner, RecoveryAmount, Owner, EquipmentCard);
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        if (equipment == null)
        {
            return;
        }

        await CreatureCmd.GainBlock(equipment.Owner.Creature, equipment.DynamicVars.Block.BaseValue, ValueProp.Move, play);
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("Amount", RecoveryAmount);
    }
}
