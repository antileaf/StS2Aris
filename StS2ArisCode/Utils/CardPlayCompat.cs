using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace StS2Aris.StS2ArisCode.Utils;

public static class CardPlayCompat
{
    private static readonly PropertyInfo? PlayerProperty = typeof(CardPlay).GetProperty("Player");

    public static Player GetPlayer(this CardPlay cardPlay)
    {
        return PlayerProperty?.GetValue(cardPlay) as Player ?? cardPlay.Card.Owner;
    }
}
