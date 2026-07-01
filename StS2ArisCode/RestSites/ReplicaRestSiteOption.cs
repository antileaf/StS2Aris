using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Extensions;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.RestSites;

public sealed class ReplicaRestSiteOption(Player owner, Replica replica) : CustomRestSiteOption(owner)
{
    private static readonly string IconAssetPath = "option_replica.png".CharacterUiPath();

    public override string OptionId => "STS2ARIS_REPLICA";

    public override string CustomIconPath => IconAssetPath;

    public override IEnumerable<string> AssetPaths => [IconAssetPath];

    public override bool IsEnabled => HasValidTarget(Owner, replica);

    public override LocString Description => new("rest_site_ui", IsEnabled
        ? "OPTION_STS2ARIS_REPLICA.description"
        : "OPTION_STS2ARIS_REPLICA.descriptionDisabled");

    public override async Task<bool> OnSelect()
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (replica.QuestRemaining <= 0)
        {
            replica.QuestRemaining = replica.QuestGoal;
        }

        replica.QuestRemaining = int.Max(0, replica.QuestRemaining - 1);
        if (replica.QuestRemaining > 0)
        {
            return true;
        }

        var rewardTargets = GetRewardTargets(Owner, replica).ToList();
        if (rewardTargets.Count == 0)
        {
            return false;
        }

        await CardPileCmd.RemoveFromDeck(replica, showPreview: false);
        var addedResults = new List<CardPileAddResult>();
        foreach (var rewardTarget in rewardTargets)
        {
            PlayerCmd.CompleteQuest(rewardTarget);
            var addedResult = await ArisQuestUtils.ApplyReplicaRewardFor(rewardTarget, replica.IsUpgraded);
            if (addedResult.HasValue)
            {
                addedResults.Add(addedResult.Value);
            }
        }

        if (addedResults.Count > 0)
        {
            CardCmd.PreviewCardPileAdd(addedResults, 2f);
        }
        return true;
    }

    public static bool HasValidTarget(Player player, Replica replica)
    {
        return GetRewardTargets(player, replica).Any();
    }

    private static IEnumerable<CardModel> GetRewardTargets(Player player, Replica replica)
    {
        return PileType.Deck.GetPile(player).Cards.Where(card => IsValidTarget(card, replica));
    }

    private static bool IsValidTarget(CardModel card, Replica replica)
    {
        return card != replica && ArisQuestUtils.HasSelectableReplicaReward(card);
    }
}
