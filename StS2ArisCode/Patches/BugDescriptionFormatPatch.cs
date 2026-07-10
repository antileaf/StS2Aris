using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Aris.StS2ArisCode.Cards;
using StS2Aris.StS2ArisCode.Keywords;

namespace StS2Aris.StS2ArisCode.Patches;

public static class BugDescriptionFormatPatch
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
        {
            return;
        }

        DescriptionOverrides.CustomizeDescriptionPost += MergeBugKeywordLines;
        _registered = true;
    }

    private static void MergeBugKeywordLines(CardModel card, Creature? target, ref string description)
    {
        if (card is not Bug || !card.Keywords.Contains(ArisKeywords.Output) || !card.ShouldRetainThisTurn)
        {
            return;
        }

        var lines = description.Split('\n');
        if (lines.Length < 2 || !IsKeywordLinePair(lines[0], lines[1]))
        {
            return;
        }

        description = string.Join('\n', new[] { $"{lines[0]} {lines[1]}" }.Concat(lines.Skip(2)));
    }

    private static bool IsKeywordLinePair(string first, string second)
    {
        return IsOutputLine(first) && IsRetainLine(second) ||
               IsRetainLine(first) && IsOutputLine(second);
    }

    private static bool IsOutputLine(string line)
    {
        return line == KeywordLine("STS2ARIS-OUTPUT");
    }

    private static bool IsRetainLine(string line)
    {
        return line == KeywordLine("RETAIN");
    }

    private static string KeywordLine(string locKeyPrefix)
    {
        var title = new LocString("card_keywords", $"{locKeyPrefix}.title").GetFormattedText();
        var period = new LocString("card_keywords", "PERIOD").GetRawText();
        return $"[gold]{title}[/gold]{period}";
    }
}
