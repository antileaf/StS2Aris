using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class HeroSword() : StS2ArisEquipmentCard(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    private static readonly Dictionary<Player, int> ClassChangeHitBonuses = new();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Equipment),
        HoverTipFactory.FromKeyword(ArisKeywords.ClassChange),
        HoverTipFactory.FromKeyword(ArisKeywords.Job)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move)
    ];

    public static void ResetCombatCounters()
    {
        ClassChangeHitBonuses.Clear();
    }

    public static int GetClassChangeHits(Player? player)
    {
        return player != null && ClassChangeHitBonuses.TryGetValue(player, out var bonus)
            ? 1 + bonus
            : 1;
    }

    public static void IncreaseClassChangeHits(Player? player, int amount)
    {
        if (player == null || amount <= 0)
        {
            return;
        }

        ClassChangeHitBonuses[player] = ClassChangeHitBonuses.GetValueOrDefault(player) + amount;
    }

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobHeroPower>();
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        var showHits = IsMutable
                       && Owner?.Creature.CombatState != null
                       && Pile?.Type != PileType.Deck;
        description.Add("ShowHits", showHits);
        description.Add("Hits", showHits ? GetClassChangeHits(Owner) : 1);
        description.Add("Increase", GetUnequipIncreaseForDescription());
    }

    private int GetUnequipIncreaseForDescription()
    {
        if (!IsMutable || Owner?.Creature.CombatState == null)
        {
            return 1;
        }

        return ArisEquipment.GetCurrentJob(Owner) is JobHeroPower heroPower && heroPower.EquipmentCard == this
            ? heroPower.UnequipIncrease
            : 1;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
