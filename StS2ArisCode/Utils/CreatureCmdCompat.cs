using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Aris.StS2ArisCode.Utils;

public static class CreatureCmdCompat
{
    private static readonly MethodInfo? DamageWithDealer = typeof(CreatureCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(method =>
        {
            if (method.Name != nameof(CreatureCmd.Damage))
            {
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();
            return parameters.Length == 6
                   && parameters[1].ParameterType == typeof(Creature)
                   && parameters[4].ParameterType == typeof(Creature);
        });

    private static readonly MethodInfo? DamageWithCardPlay = typeof(CreatureCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(method =>
        {
            if (method.Name != nameof(CreatureCmd.Damage))
            {
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();
            return parameters.Length == 6
                   && parameters[1].ParameterType == typeof(Creature)
                   && parameters[4].ParameterType == typeof(CardModel);
        });

    public static Task<IEnumerable<DamageResult>> DamageFromCard(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature dealer,
        CardModel cardSource,
        CardPlay? cardPlay)
    {
        if (DamageWithCardPlay != null)
        {
            return Invoke(DamageWithCardPlay, choiceContext, target, amount, props, cardSource, cardPlay);
        }

        if (DamageWithDealer != null)
        {
            return Invoke(DamageWithDealer, choiceContext, target, amount, props, dealer, cardSource);
        }

        throw new MissingMethodException(typeof(CreatureCmd).FullName, nameof(CreatureCmd.Damage));
    }

    private static Task<IEnumerable<DamageResult>> Invoke(MethodInfo method, params object?[] args)
    {
        try
        {
            return (Task<IEnumerable<DamageResult>>)method.Invoke(null, args)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }
}
