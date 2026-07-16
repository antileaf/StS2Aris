using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(TokenCardPool))]
public class HoshinoTank() : StS2ArisEquipmentCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromKeyword(ArisKeywords.Equipment);
            yield return HoverTipFactory.FromKeyword(ArisKeywords.ClassChange);
            yield return HoverTipFactory.FromKeyword(ArisKeywords.Job);

            var expert = HoshinoReflection.GetKeyword("Expert");
            if (expert != null)
            {
                yield return HoverTipFactory.FromKeyword(expert.Value);
            }

            var reload = HoshinoReflection.GetKeyword("Reload");
            if (reload != null)
            {
                yield return HoverTipFactory.FromKeyword(reload.Value);
            }
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("ExpertAmount", 5m)];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobHoshinoTankPower>();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExpertAmount"].UpgradeValueBy(3m);
    }
}
