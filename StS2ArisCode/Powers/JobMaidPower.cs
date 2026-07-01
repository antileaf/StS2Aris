using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobMaidPower : ArisJobPower
{
    public override string AnimationSuffix => "Hero";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != PlayerOwner || CombatState == null)
        {
            return;
        }

        Flash();
        await CleanUp.CreateInHand(player, CombatState, EquipmentCard?.IsUpgraded ?? false);
    }
}
