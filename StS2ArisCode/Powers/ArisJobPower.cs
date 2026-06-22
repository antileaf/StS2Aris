using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace StS2Aris.StS2ArisCode.Powers;

public abstract class ArisJobPower : StS2ArisPower
{
    public CardModel? EquipmentCard { get; set; }
    public abstract string AnimationSuffix { get; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

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

    protected int LevelBonus => Owner.GetPower<LevelUpPower>()?.Amount ?? 0;
    protected Player? PlayerOwner => Owner.Player;
    protected decimal EquipmentStrengthAmount => EquipmentCard?.DynamicVars["StrengthPower"].BaseValue ?? 0m;

    public void FlashJob()
    {
        Flash();
    }

    public virtual Task OnClassChange(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
}
