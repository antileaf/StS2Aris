using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using StS2Aris.StS2ArisCode.Character;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class FinalRelease() : StS2ArisCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Replay", 1m)];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1),
            card => card != this && card.Type is CardType.Attack or CardType.Skill,
            this)).FirstOrDefault();

        if (selected == null)
        {
            return;
        }

        if (selected.IsUpgradable)
        {
            CardCmd.Upgrade(selected);
        }

        CardCmd.ApplyKeyword(selected, CardKeyword.Exhaust);
        selected.BaseReplayCount += DynamicVars["Replay"].IntValue;

        if (selected is GameScenario scenario)
        {
            scenario.ScenarioExhaust = true;
            scenario.ScenarioFinalRelease = true;
            scenario.RefreshGeneratedValues();
        }
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
