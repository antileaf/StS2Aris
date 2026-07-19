using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;

namespace StS2Aris.StS2ArisCode.Relics;

public sealed class KeiClassRelic : ClassAltarRelic
{
    protected override CardPoolModel? RewardCardPool => null;
    protected override CardModel CreateEquipment(Player owner) => owner.RunState.CreateCard<LuminousNova>(owner);
}
