using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Relics;
public class ArisBaseRelic : StS2ArisRelic
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ChargePower>(1m)];
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            await PowerCmd.Apply<ChargePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, null);
        }
    }
}

