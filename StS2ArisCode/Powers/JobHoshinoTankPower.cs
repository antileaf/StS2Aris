using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobHoshinoTankPower : ArisJobPower
{
    public override string AnimationSuffix => "Hoshino";

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var player = equipment?.Owner;
        if (equipment == null || player == null)
        {
            return;
        }

        await HoshinoReflection.ApplyExpert(
            choiceContext,
            player,
            equipment.DynamicVars["ExpertAmount"].BaseValue,
            player.Creature,
            equipment);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        var player = PlayerOwner;
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.IsDead || player == null)
        {
            return;
        }

        FlashJob();
        for (var i = 0; i < EffectApplications; i++)
        {
            await HoshinoReflection.Reload(choiceContext, player);
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
        description.Add("Reloads", EffectApplications);
    }
}
