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
        if (ArisEquipment.ShouldTriggerClassChange(Owner, this))
        {
            await OnClassChange(choiceContext, play);
            await ArisHook.OnClassChanged(choiceContext, Owner, this);
        }

        await ArisEquipment.Equip(choiceContext, this, CreateJobPower());
    }

    protected virtual Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected override PileType GetResultPileTypeForCardPlay()
    {
        return PileType.None;
    }
}
