using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using StS2Aris;

namespace StS2Aris.StS2ArisCode.Extensions;

public static class AttackCommandExtensions
{
    private static readonly MethodInfo? FromCardWithCardPlay = typeof(AttackCommand)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == nameof(AttackCommand.FromCard) &&
            method.GetParameters().Length == 2);

    private static readonly MethodInfo? FromCardWithoutCardPlay = typeof(AttackCommand)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == nameof(AttackCommand.FromCard) &&
            method.GetParameters().Length == 1);

    public static AttackCommand FromCardCompat(this AttackCommand command, CardModel card, CardPlay? cardPlay)
    {
        if (FromCardWithCardPlay != null)
        {
            return (AttackCommand)FromCardWithCardPlay.Invoke(command, [card, cardPlay])!;
        }

        if (FromCardWithoutCardPlay != null)
        {
            return (AttackCommand)FromCardWithoutCardPlay.Invoke(command, [card])!;
        }

        throw new MissingMethodException(typeof(AttackCommand).FullName, nameof(AttackCommand.FromCard));
    }

    public static AttackCommand WithArisHitFx(this AttackCommand command, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        command.WithHitFx(vfx, sfx: null, tmpSfx);
        if (sfx == null)
            return command;

        return command.BeforeDamage(() =>
        {
            StS2ArisMain.PlayAttackSfx(sfx);
            return Task.CompletedTask;
        });
    }
}
