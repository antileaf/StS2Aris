using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class EmergencyPowerPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (amount > 0 && card.Owner.Creature == Owner && card.Owner.PlayerCombatState?.Energy == 0)
        {
            await OnEnergyBecameZero(card.Owner);
        }
    }

    public static async Task OnEnergyBecameZero(Player player)
    {
        if (player.Creature.GetPower<EmergencyPowerPower>() is not { } power ||
            player.Creature.CombatState == null ||
            CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }

        power.Flash();
        await Shock.CreateInHand(player, power.Amount, player.Creature.CombatState);
    }
}
