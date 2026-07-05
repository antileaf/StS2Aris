using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Hooks;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobMasteryPower : StS2ArisPower, IOnClassChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnClassChanged(PlayerChoiceContext choiceContext, Player player, CardModel? source)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner, Amount,  ValueProp.Unpowered, null);
    }
}
