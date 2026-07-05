using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;
[Pool(typeof(StS2ArisCardPool))]
public class WizardHat() : StS2ArisEquipmentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Equipment),
        HoverTipFactory.FromKeyword(ArisKeywords.ClassChange),
        HoverTipFactory.FromKeyword(ArisKeywords.Job),
        HoverTipFactory.FromPower<EndTurnBlockPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Magic", 3m)
    ];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobWizardPower>();
    }

    protected override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var powerCards = drawPile.Cards
            .Where(static card => card.Type == CardType.Power)
            .ToList();

        CardModel? selected;
        if (IsUpgraded)
        {
            selected = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                drawPile,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1),
                static card => card.Type == CardType.Power)).FirstOrDefault();
        }
        else
        {
            selected = powerCards.TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        }

        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Magic"].UpgradeValueBy(1m);
    }
}




