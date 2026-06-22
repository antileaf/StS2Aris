using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Aris.StS2ArisCode.CardModels;

public interface IOverload
{
    Task OnOverload(PlayerChoiceContext choiceContext, CardPlay play);
}
