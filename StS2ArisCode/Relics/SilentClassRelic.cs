using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Relics;

public sealed class SilentClassRelic : ClassAltarRelic
{
    protected override CardPoolModel RewardCardPool => ModelDb.CardPool<SilentCardPool>();
    protected override CardModel CreateEquipment(Player owner) => owner.RunState.CreateCard<Rogue>(owner);
}
