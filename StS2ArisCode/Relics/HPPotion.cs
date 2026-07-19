using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Relics;
public class HPPotion : StS2ArisRelic
{
    private bool _usedThisCombat;
    private CardModel? _pendingCardSource;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            _usedThisCombat = false;
            _pendingCardSource = null;
            Status = RelicStatus.Active;
        }

        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        ICombatState? combatState = Owner.Creature.CombatState;
        if (_usedThisCombat || combatState == null || combatState.CurrentSide != Owner.Creature.Side
            || target != Owner.Creature || result.TotalDamage <= 0)
        {
            return Task.CompletedTask;
        }

        _usedThisCombat = true;
        Status = RelicStatus.Normal;
        if (cardSource?.Pile?.Type == PileType.Play)
        {
            _pendingCardSource = cardSource;
            return Task.CompletedTask;
        }

        UpgradeHand();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_pendingCardSource != cardPlay.Card)
        {
            return Task.CompletedTask;
        }

        _pendingCardSource = null;
        UpgradeHand();
        return Task.CompletedTask;
    }

    private void UpgradeHand()
    {
        Flash();
        foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards.Where(card => card.IsUpgradable).ToList())
        {
            CardCmd.Upgrade(card);
        }
    }
}
