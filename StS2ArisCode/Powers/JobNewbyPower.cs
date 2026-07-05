using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobNewbyPower : ArisJobPower
{
    public override string AnimationSuffix => "Newby";

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (PlayerOwner == null)
        {
            return;
        }

        Flash();
        await CardPileCmd.Draw(choiceContext, Amount + LevelBonus, PlayerOwner);
    }
}
