using System.Reflection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace StS2Aris.StS2ArisCode.Utils;

public static class CardCreationOptionsCompat
{
    private static readonly MethodInfo? WithCardPoolsMethod = typeof(CardCreationOptions)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(method => method.Name == "WithCardPools")
        .FirstOrDefault(method =>
        {
            ParameterInfo[] parameters = method.GetParameters();
            return parameters.Length is 1 or 2
                   && parameters[0].ParameterType == typeof(IEnumerable<CardPoolModel>);
        });

    public static CardCreationOptions WithCardPools(
        CardCreationOptions options,
        IEnumerable<CardPoolModel> pools)
    {
        if (WithCardPoolsMethod == null)
        {
            throw new MissingMethodException(typeof(CardCreationOptions).FullName, "WithCardPools");
        }

        object?[] arguments = WithCardPoolsMethod.GetParameters().Length == 1
            ? [pools]
            : [pools, options.CardPoolFilter];

        try
        {
            return (CardCreationOptions)WithCardPoolsMethod.Invoke(options, arguments)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }
}
