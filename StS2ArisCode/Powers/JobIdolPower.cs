using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace StS2Aris.StS2ArisCode.Powers;

public sealed class JobIdolPower : ArisJobPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 3m)];
    public override string AnimationSuffix => "Idol";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (PlayerOwner == null || player != PlayerOwner || CombatState == null)
        {
            return;
        }

        var options = PlayerOwner.Character.CardPool.GetUnlockedCards(PlayerOwner.UnlockState, PlayerOwner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity == CardRarity.Common && c.GetType() != EquipmentCard?.GetType())
            .ToList();
        if (options.Count == 0)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount + LevelBonus; i++)
        {
            var canonicalCard = PlayerOwner.RunState.Rng.CombatCardGeneration.NextItem(options);
            if (canonicalCard == null)
            {
                continue;
            }

            var card = CombatState.CreateCard(canonicalCard, PlayerOwner);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, PlayerOwner);
        }
    }

    public override async Task OnClassChange(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var equipment = EquipmentCard;
        var player = equipment?.Owner ?? PlayerOwner;
        if (equipment == null || player == null)
        {
            return;
        }

        var drawAmount = equipment.DynamicVars["Magic"].IntValue;
        await CardPileCmd.Draw(choiceContext, drawAmount, player);
    }
}
