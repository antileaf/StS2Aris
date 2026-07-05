using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Hooks;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

public abstract class StS2ArisEquipmentCard(int cost, CardType type, CardRarity rarity, TargetType targetType)
    : StS2ArisCard(cost, type, rarity, targetType), IArisEquipmentCard
{
    public abstract ArisJobPower CreateJobPower();

    protected static T MakeJobPower<T>() where T : ArisJobPower
    {
        return (T)ModelDb.Power<T>().ToMutable();
    }

    protected sealed override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.PlayIndex > 0)
        {
            return;
        }

        var nextJob = CreateJobPower();
        if (ArisEquipment.ShouldTriggerClassChange(Owner, nextJob))
        {
            await OnClassChange(choiceContext, play);
            if (Owner.Creature.IsDead)
            {
                return;
            }

            await ArisHook.OnClassChanged(choiceContext, Owner, this);
            if (Owner.Creature.IsDead)
            {
                return;
            }
        }

        await ArisEquipment.Equip(choiceContext, this, nextJob);
    }

    protected virtual Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
    {
        return (PileType.None, CardPilePosition.Bottom);
    }
}
