using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobRoguePower : ArisJobPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 1m)];
    public override string AnimationSuffix => "Rogue";

    private decimal ShockAmount => Amount + LevelBonus;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.EnergyCost.CostsX || cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
        {
            return;
        }

        var player = PlayerOwner;
        var targets = CombatState?.HittableEnemies.ToList();
        if (player == null || targets == null || targets.Count == 0)
        {
            return;
        }

        Flash();
        var target = player.RunState.Rng.CombatTargets.NextItem(targets);
        if (target != null)
        {
            await PowerCmd.Apply<ShockPower>(choiceContext, target, ShockAmount, Owner, EquipmentCard);
        }
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("Amount", ShockAmount);
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var combatState = equipment?.CombatState ?? equipment?.Owner.Creature.CombatState;
        if (equipment == null || combatState == null)
        {
            return;
        }

        var amount = equipment.DynamicVars["Magic"].IntValue;
        await PowerCmd.Apply<WeakPower>(choiceContext, combatState.HittableEnemies, amount, equipment.Owner.Creature, equipment);
        await PowerCmd.Apply<ShockPower>(choiceContext, combatState.HittableEnemies, amount, equipment.Owner.Creature, equipment);
    }
}
