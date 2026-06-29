using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Aris.StS2ArisCode.Hooks;

public interface IOnClassChanged
{
    Task OnClassChanged(PlayerChoiceContext choiceContext, Player player, CardModel? source);
}
