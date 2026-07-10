using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Aris.StS2ArisCode.CardModels;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(StS2ArisCardPool))]
public class Bug() : StS2ArisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IArisOutputCard, IOverload
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [ArisKeywords.Output, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ArisKeywords.Output),
        HoverTipFactory.FromKeyword(ArisKeywords.Overload),
        ArisHoverTips.ChargePower()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4, ValueProp.Move),
        new PowerVar<ChargePower>(1m)
    ];

    protected override async Task OnArisPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, play);
        await PowerCmd.Apply<ChargePower>(choiceContext, Owner.Creature, DynamicVars["ChargePower"].IntValue, Owner.Creature, this);
    }

    public async Task OnOverload(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1),
            card => card != this,
            this)).FirstOrDefault();

        if (selected?.CardScope == null)
        {
            return;
        }

        await CardCmd.Transform(selected, CreateReplacementBug(selected));
    }

    private Bug CreateReplacementBug(CardModel selected)
    {
        var replacement = selected.CardScope!.CreateCard<Bug>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(replacement);
        }

        return replacement;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
