using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2Aris.StS2ArisCode.Character;
using StS2Aris.StS2ArisCode.Keywords;
using StS2Aris.StS2ArisCode.Powers;

namespace StS2Aris.StS2ArisCode.Cards;

[Pool(typeof(TokenCardPool))]
public class StarThrone() : StS2ArisEquipmentCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromKeyword(ArisKeywords.Equipment);
            yield return HoverTipFactory.FromKeyword(ArisKeywords.ClassChange);
            yield return HoverTipFactory.FromKeyword(ArisKeywords.Job);
            yield return HoverTipFactory.FromKeyword(ArisKeywords.Overload);
            foreach (var tip in HoverTipFactory.FromForge())
            {
                yield return tip;
            }
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StarsVar(3),
        new ForgeVar(7)
    ];

    public override ArisJobPower CreateJobPower()
    {
        return MakeJobPower<JobKingPower>();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Stars.UpgradeValueBy(1m);
    }
}
