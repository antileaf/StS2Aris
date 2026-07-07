using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class RestoreStrPower : StS2ArisPower
{
    [SavedProperty]
    public int TurnsRemaining { get; set; } = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("TurnsRemaining", TurnsRemaining)];

    public override LocString Description => new("powers", DescriptionLocKey);

    protected override string SmartDescriptionLocKey => DescriptionLocKey.Replace(".description", ".smartDescription");

    private string DescriptionLocKey => TurnsRemaining <= 1
        ? $"{Id.Entry}.description.last"
        : $"{Id.Entry}.description";

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.IsDead)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
        TurnsRemaining--;
        RefreshTurnsRemaining();
        if (TurnsRemaining <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }

    private void RefreshTurnsRemaining()
    {
        if (DynamicVars.TryGetValue("TurnsRemaining", out var turnsRemaining))
        {
            turnsRemaining.BaseValue = TurnsRemaining;
        }

        InvokeDisplayAmountChanged();
    }
}
