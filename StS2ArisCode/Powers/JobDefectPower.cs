using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobDefectPower : ArisJobPower
{
    public override string AnimationSuffix => "Defect";

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach (var tip in base.ExtraHoverTips)
            {
                yield return tip;
            }

            yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
            yield return HoverTipFactory.FromOrb<LightningOrb>();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner ||
            cardPlay.Card.EnergyCost.CostsX ||
            cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.All) != 0 ||
            PlayerOwner == null)
        {
            return;
        }

        Flash();
        for (var i = 0; i < EffectApplications; i++)
        {
            await OrbCmd.Channel<LightningOrb>(choiceContext, PlayerOwner);
        }
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var player = equipment?.Owner;
        if (equipment == null || player == null)
        {
            return;
        }

        await OrbCmd.AddSlots(player, equipment.DynamicVars["OrbSlots"].IntValue);
    }

    public override void AddDumbVariablesToPowerDescription(LocString description)
    {
        base.AddDumbVariablesToPowerDescription(description);
        description.Add("ChannelAmount", EffectApplications);
    }
}
