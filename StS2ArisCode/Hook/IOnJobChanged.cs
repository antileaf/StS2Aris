using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Hooks;

public interface IOnJobChanged
{
    Task OnJobChanged(PlayerChoiceContext choiceContext, Player player, ArisJobPower? previousJob, ArisJobPower? currentJob, CardModel? source);
}
