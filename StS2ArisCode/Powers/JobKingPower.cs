using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Hooks;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobKingPower : ArisJobPower, IOnOverloadTriggered
{
    public override string AnimationSuffix => "Regent";

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        if (equipment?.Owner == null)
        {
            return;
        }

        await PlayerCmd.GainStars(equipment.DynamicVars.Stars.BaseValue, equipment.Owner);
    }

    public async Task OnOverloadTriggered(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? source)
    {
        var equipment = EquipmentCard;
        if (player.Creature != Owner || equipment == null)
        {
            return;
        }

        FlashJob();
        await ForgeCmd.Forge(equipment.DynamicVars.Forge.IntValue, player, equipment);
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("Forge", EquipmentCard?.DynamicVars.Forge.BaseValue ?? 5m);
    }
}
