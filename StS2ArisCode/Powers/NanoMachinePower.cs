using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Powers;
public sealed class NanoMachinePower : StS2ArisPower, BaseLib.Patches.Localization.IAddDumbVariablesToPowerDescription
{
    private const int DefaultLossDivisor = 2;

    [SavedProperty]
    public int TurnsRemaining { get; set; } = 2;

    [SavedProperty]
    public int AbsorbedDamage { get; set; }

    [SavedProperty]
    public int LossDivisor { get; set; } = DefaultLossDivisor;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("TurnsRemaining", TurnsRemaining);
    }

    public void DelayHpLossTurn()
    {
        TurnsRemaining++;
        RefreshDisplay();
        Flash();
    }

    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || amount <= 0 || props.HasFlag(ValueProp.Unblockable))
        {
            return amount;
        }

        AbsorbedDamage += (int)Math.Ceiling(amount);
        SetAmount(GetHpLossAmount());
        Flash();
        return 0;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.IsDead)
        {
            return;
        }

        TurnsRemaining--;
        if (TurnsRemaining > 0)
        {
            RefreshDisplay();
            return;
        }

        var hpLoss = Amount;
        if (hpLoss > 0)
        {
            await CreatureCmd.Damage(choiceContext, Owner, hpLoss, DamageProps.nonCardHpLoss, Applier, null, null);
        }

        await PowerCmd.Remove(this);
    }

    private int GetHpLossAmount()
    {
        return LossDivisor <= 1 ? AbsorbedDamage : (int)Math.Ceiling(AbsorbedDamage / (decimal)LossDivisor);
    }

    private void RefreshDisplay()
    {
        InvokeDisplayAmountChanged();
    }
}
