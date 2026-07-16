using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace StS2Aris.StS2ArisCode.Mechanics;

public static class HoshinoReflection
{
    private const string ExpertPowerTypeName = "StS2Hoshino.StS2HoshinoCode.Powers.ExpertPower";
    private const string HoshinoKeywordsTypeName = "StS2Hoshino.StS2HoshinoCode.Keywords.HoshinoKeywords";

    public static async Task ApplyExpert(PlayerChoiceContext choiceContext, Player player, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var expertType = FindType(ExpertPowerTypeName);
        if (expertType == null)
        {
            return;
        }

        var canonical = ModelDb.GetByIdOrNull<PowerModel>(ModelDb.GetId(expertType));
        if (canonical == null)
        {
            return;
        }

        await PowerCmd.Apply(choiceContext, canonical.ToMutable(), player.Creature, amount, applier, cardSource);
    }

    public static async Task Reload(PlayerChoiceContext choiceContext, Player player)
    {
        var reloadType = FindType("ReloadCmd");
        var method = reloadType?.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
        if (method == null)
        {
            return;
        }

        var result = method.Invoke(null, [choiceContext, player, -1, false]);
        if (result is Task task)
        {
            await task;
        }
    }

    public static CardKeyword? GetKeyword(string fieldName)
    {
        var keywordsType = FindType(HoshinoKeywordsTypeName);
        var field = keywordsType?.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        return field?.GetValue(null) is CardKeyword keyword ? keyword : null;
    }

    private static Type? FindType(string fullNameOrName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(fullNameOrName, false);
            if (type != null)
            {
                return type;
            }

            type = GetLoadableTypes(assembly).FirstOrDefault(candidate => candidate.Name == fullNameOrName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type != null)!;
        }
        catch
        {
            return [];
        }
    }
}
