using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobWizardPower : ArisJobPower
{
    private const string RepairAmountKey = "RepairAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Amount", 2m),
        new DynamicVar(RepairAmountKey, 2m)
    ];

    public override string AnimationSuffix => "Necromancer";

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this || power is LevelUpPower && power.Owner == Owner)
        {
            RefreshRepairAmount();
        }

        var target = power.Owner;
        if (applier != Owner || target.Side == Owner.Side || power.Type != PowerType.Debuff)
        {
            return;
        }

        Flash();
        var repairAmount = EquipmentCard?.DynamicVars["Magic"].IntValue ?? DynamicVars["Amount"].IntValue;
        await CreatureCmd.GainBlock(Owner, repairAmount * EffectApplications, ValueProp.Unpowered, null);
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        RefreshRepairAmount();
        return Task.CompletedTask;
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var player = equipment?.Owner;
        var combatState = equipment?.CombatState ?? player?.Creature.CombatState;
        if (equipment == null || player == null || combatState == null)
        {
            return;
        }

        var amount = equipment.DynamicVars.Cards.IntValue;
        List<Soul> cards = Soul.Create(player, amount, combatState).ToList();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, player, CardPilePosition.Random));
    }

    private void RefreshRepairAmount()
    {
        if (DynamicVars.TryGetValue(RepairAmountKey, out var repairAmount))
        {
            var baseAmount = EquipmentCard?.DynamicVars["Magic"].IntValue ?? DynamicVars["Amount"].IntValue;
            repairAmount.BaseValue = baseAmount * EffectApplications;
        }

        InvokeDisplayAmountChanged();
    }
}
