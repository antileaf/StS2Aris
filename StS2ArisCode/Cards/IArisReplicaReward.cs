using MegaCrit.Sts2.Core.Entities.Cards;

namespace StS2Aris.StS2ArisCode.Cards;

public interface IArisReplicaReward
{
    Task<CardPileAddResult?> ApplyReplicaReward(bool forceUpgrade);
}
