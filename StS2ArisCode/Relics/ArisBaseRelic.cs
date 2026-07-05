using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using StS2Aris.StS2ArisCode.Hooks;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Relics;
public class ArisBaseRelic : StS2ArisRelic, IOnClassChanged
{
    private bool _classChangeTriggeredThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ChargePower>(1m)];
    public override RelicRarity Rarity => RelicRarity.Starter;

    [SavedProperty]
    public bool ClassChangeTriggeredThisCombat
    {
        get => _classChangeTriggeredThisCombat;
        set
        {
            AssertMutable();
            _classChangeTriggeredThisCombat = value;
            Status = _classChangeTriggeredThisCombat ? RelicStatus.Normal : RelicStatus.Active;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            ClassChangeTriggeredThisCombat = false;
        }

        return Task.CompletedTask;
    }

    public async Task OnClassChanged(PlayerChoiceContext choiceContext, Player player, CardModel? source)
    {
        if (player != Owner)
        {
            return;
        }

        if (ClassChangeTriggeredThisCombat)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, null);
        ClassChangeTriggeredThisCombat = true;
    }
    
    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<ArisBaseRelicPlus>();
    }
}
