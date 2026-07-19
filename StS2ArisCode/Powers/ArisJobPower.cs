using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Patches.Localization;
using StS2Aris.StS2ArisCode.Formatters;

namespace StS2Aris.StS2ArisCode.Powers;

public abstract class ArisJobPower : StS2ArisPower, IAddDumbVariablesToPowerDescription
{
    public CardModel? EquipmentCard { get; set; }
    public abstract string AnimationSuffix { get; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (EquipmentCard != null)
            {
                yield return HoverTipFactory.FromCard(EquipmentCard);
            }
        }
    }

    protected int EffectApplications => Math.Max(1, Amount + LevelUpAmount);
    protected int LevelBonus => EffectApplications - 1;
    protected Player? PlayerOwner => Owner?.Player;
    protected decimal EquipmentStrengthAmount =>
        EquipmentCard?.DynamicVars.TryGetValue("StrengthPower", out var strength) == true
            ? strength.BaseValue
            : 0m;
    private int LevelUpAmount => (int)Owner.Powers.OfType<LevelUpPower>().Sum(power => power.Amount);

    public void FlashJob()
    {
        Flash();
    }

    public virtual Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnUnequipped(PlayerChoiceContext choiceContext, PileType resultPileType)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnLevelUpChanged(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public virtual void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("StrengthAmount", EquipmentStrengthAmount * EffectApplications);
        description.Add("chargeIcon", ChargeIconFormatter.Icon);
    }
}
