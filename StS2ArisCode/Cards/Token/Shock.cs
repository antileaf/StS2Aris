using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(TokenCardPool))]
public class Shock() : StS2ArisCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    private bool IsUncontrollable =>
        IsMutable && Owner?.Creature.GetPower<UncontrollablePower>() != null;

    public override TargetType TargetType =>
        IsUncontrollable
            ? TargetType.RandomEnemy
            : TargetType.AnyEnemy;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(ArisKeywords.Shock)];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move),
        new PowerVar<ShockPower>(2m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target == null && CombatState != null)
        {
            target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        }

        if (target == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play).Targeting(target).Execute(choiceContext);
        await PowerCmd.Apply<ShockPower>(choiceContext, target, DynamicVars["ShockPower"].IntValue, Owner.Creature, this);
        if (play.Target == null && Enchantment is Inky inky && target.IsAlive)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, target, inky.DynamicVars.Weak.BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["ShockPower"].UpgradeValueBy(1m);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        var key = IsUncontrollable ? "randomDescription" : "normalDescription";
        var shockDescription = new LocString("cards", $"{Id.Entry}.{key}");
        DynamicVars.AddTo(shockDescription);
        base.AddExtraArgsToDescription(shockDescription);
        description.Add("ShockDescription", shockDescription.GetFormattedText());
    }

    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState, bool upgraded = false)
    {
        return (await CreateInHand(owner, 1, combatState, upgraded)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState, bool upgraded = false)
    {
        if (count == 0 || CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();

        List<CardModel> shocks = [];
        for (int i = 0; i < count; i++)
        {
            var shock = combatState.CreateCard<Shock>(owner);
            if (upgraded)
                CardCmd.Upgrade(shock);
            shocks.Add(shock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(shocks, PileType.Hand, owner);
        return shocks;
    }
}
