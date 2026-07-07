using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Extensions;
using StS2Aris.StS2ArisCode.Mechanics;
using StS2Aris.StS2ArisCode.Utils;

namespace StS2Aris.StS2ArisCode.RestSites;

public sealed class ReplicaRestSiteOption(Player owner, ItemCopyBug itemCopyBug) : CustomRestSiteOption(owner)
{
    private static readonly string IconAssetPath = "option_replica.png".CharacterUiPath();

    public override string OptionId => "STS2ARIS_REPLICA";

    public override string CustomIconPath => IconAssetPath;

    public override IEnumerable<string> AssetPaths => [IconAssetPath];

    public override bool IsEnabled => HasValidTarget(Owner, itemCopyBug);

    public override LocString Description => new("rest_site_ui", IsEnabled
        ? "OPTION_STS2ARIS_REPLICA.description"
        : "OPTION_STS2ARIS_REPLICA.descriptionDisabled");

    public override async Task<bool> OnSelect()
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (itemCopyBug.QuestRemaining <= 0)
        {
            itemCopyBug.QuestRemaining = itemCopyBug.QuestGoal;
        }

        itemCopyBug.QuestRemaining = int.Max(0, itemCopyBug.QuestRemaining - 1);
        if (itemCopyBug.QuestRemaining > 0)
        {
            return true;
        }

        var rewardTargets = GetRewardTargets(Owner, itemCopyBug).ToList();
        if (rewardTargets.Count == 0)
        {
            return false;
        }

        await CardPileCmd.RemoveFromDeck(itemCopyBug, showPreview: false);
        var addedResults = new List<CardPileAddResult>();
        foreach (var rewardTarget in rewardTargets)
        {
            ArisQuestProgress.MarkCompleted(rewardTarget);
            var addedResult = await ArisQuestUtils.ApplyReplicaRewardFor(rewardTarget, itemCopyBug.IsUpgraded);
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

    public static bool HasValidTarget(Player player, ItemCopyBug itemCopyBug)
    {
        return GetRewardTargets(player, itemCopyBug).Any();
    }

    private static IEnumerable<CardModel> GetRewardTargets(Player player, ItemCopyBug itemCopyBug)
    {
        return PileType.Deck.GetPile(player).Cards.Where(card => IsValidTarget(card, itemCopyBug));
    }

    private static bool IsValidTarget(CardModel card, ItemCopyBug itemCopyBug)
    {
        return card != itemCopyBug && ArisQuestUtils.HasSelectableReplicaReward(card);
    }
}
