using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Hooks;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class WeaponMasterPower : StS2ArisPower, IOnJobChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnJobChanged(PlayerChoiceContext choiceContext, Player player, ArisJobPower? previousJob, ArisJobPower currentJob, CardModel? source)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}
