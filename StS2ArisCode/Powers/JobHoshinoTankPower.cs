using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using StS2Aris.StS2ArisCode.Mechanics;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobHoshinoTankPower : ArisJobPower
{
    public override string AnimationSuffix => "Hoshino";

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (PlayerOwner == null || EquipmentCard == null)
        {
            return;
        }

        await HoshinoReflection.ApplyExpert(
            choiceContext,
            PlayerOwner,
            EquipmentCard.DynamicVars["ExpertAmount"].BaseValue,
            Owner,
            EquipmentCard);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.IsDead || PlayerOwner == null)
        {
            return;
        }

        FlashJob();
        await HoshinoReflection.Reload(choiceContext, PlayerOwner);
    }
}
