using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace StS2Aris.StS2ArisCode.Formatters;

public class ChargeIconFormatter : IFormatter
{
    public const string Icon = "[img]res://StS2Aris/images/charui/aris_text_charge.png[/img]";

    public string Name
    {
        get => "chargeIcon";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; } = false;

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        int count = formattingInfo.CurrentValue switch
        {
            DynamicVar dynamicVar => (int)dynamicVar.PreviewValue,
            decimal decimalValue => (int)decimalValue,
            int intValue => intValue,
            string stringValue when int.TryParse(stringValue, out var parsed) => parsed,
            _ => 0
        };

        formattingInfo.Write(Format(count));
        return true;
    }

    public static string Format(int count)
    {
        count = Math.Max(0, count);
        return count is > 0 and < 4
            ? string.Concat(Enumerable.Repeat(Icon, count))
            : $"{count}{Icon}";
    }
}
