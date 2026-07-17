using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Hooks;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class BalanceBroken() : StS2ArisCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IOnOverloadTriggered
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(ArisKeywords.Overload)];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24, ValueProp.Move)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompat(this, play).TargetingAllOpponents(CombatState).Execute(choiceContext);
        }
    }

    public Task OnOverloadTriggered(PlayerChoiceContext choiceContext, Player player, CardModel? source)
    {
        if (player == Owner && Pile?.IsCombatPile == true)
        {
            EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }

        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
