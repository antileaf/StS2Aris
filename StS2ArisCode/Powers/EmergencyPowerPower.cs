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
