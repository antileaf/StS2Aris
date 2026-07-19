using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Events;

namespace StS2Aris.StS2ArisCode.Relics;

public sealed class HoshinoClassRelic : ClassAltarRelic
{
    private const string CardPoolTypeName =
        "StS2Hoshino.StS2HoshinoCode.Character.StS2HoshinoCardPool";

    protected override CardPoolModel? RewardCardPool => ClassAltarJobRegistry.FindCardPool(CardPoolTypeName);
    protected override CardModel CreateEquipment(Player owner) => owner.RunState.CreateCard<HoshinoTank>(owner);
}
