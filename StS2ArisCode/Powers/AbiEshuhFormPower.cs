using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Patches.Localization;
using StS2Aris.StS2ArisCode.Formatters;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class AbiEshuhFormPower : StS2ArisPower, IAddDumbVariablesToPowerDescription
{
    private const string DamagePerChargeKey = "DamagePerCharge";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(DamagePerChargeKey, 1m)];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars[DamagePerChargeKey].BaseValue = GetDamagePerCharge(cardSource);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0 && Amount != (int)amount && cardSource?.DynamicVars.ContainsKey(DamagePerChargeKey) == true)
        {
            DynamicVars[DamagePerChargeKey].BaseValue += GetDamagePerCharge(cardSource);
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && participants.Contains(Owner) && !Owner.IsDead)
        {
            Flash();
            await PowerCmd.Apply<ChargePower>(choiceContext, Owner, Amount, Owner, null);
        }
    }

    public decimal ModifyDamageAdditiveCompat(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource?.Type != CardType.Attack || !props.IsPoweredAttack() || Owner.Player == null)
        {
            return 0m;
        }

        var sourceCard = cardPlay?.Card ?? cardSource;
        var chargeBeforeCost = ArisCharge.Get(Owner.Player) + (sourceCard == null ? 0 : ArisCharge.GetSpent(sourceCard));
        return chargeBeforeCost * DynamicVars[DamagePerChargeKey].BaseValue;
    }

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("singleChargeIcon", ChargeIconFormatter.Icon);
        description.Add("ChargeIcons", ChargeIconFormatter.Format((int)Amount));
        description.Add("Amount", Amount);
        description.Add("DamagePerCharge", DynamicVars[DamagePerChargeKey].BaseValue);
    }

    private static decimal GetDamagePerCharge(CardModel? cardSource)
    {
        return cardSource?.DynamicVars.TryGetValue(DamagePerChargeKey, out var damagePerCharge) == true
            ? damagePerCharge.BaseValue
            : 1m;
    }
}
