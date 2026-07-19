using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobKeiPower : ArisJobPower
{
    public override string AnimationSuffix => "Kei";

    private decimal DrawAmount => EffectApplications;

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardModel? equipment = EquipmentCard;
        Player? player = equipment?.Owner;
        if (equipment == null || player == null)
        {
            return;
        }

        await PowerCmd.Apply<ChargePower>(
            choiceContext,
            player.Creature,
            equipment.DynamicVars["ChargePower"].BaseValue,
            player.Creature,
            equipment);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != PlayerOwner || Owner.IsDead)
        {
            return;
        }

        FlashJob();
        await CardPileCmd.Draw(choiceContext, DrawAmount, player);
    }

    public override Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("Cards", DrawAmount);
    }
}
