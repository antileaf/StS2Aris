using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class NeatCompressionPower : StS2ArisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        var player = Owner.Player;
        if (player == null)
        {
            return Task.CompletedTask;
        }

        for (var i = 0; i < Amount; i++)
        {
            room.AddExtraReward(player, new CardRemovalReward(player));
        }

        return Task.CompletedTask;
    }
}
