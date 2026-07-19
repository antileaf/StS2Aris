using BaseLib.Config;
using Godot;

namespace StS2Aris.StS2ArisCode.Config;

[ConfigHoverTipsByDefault]
public class ArisModConfig : SimpleModConfig
{
    private const int PreviewDelayMs = 180;
    private static float _lastAttackSfxVolumePercent;
    private static CancellationTokenSource? _previewToken;

    public ArisModConfig()
    {
        _lastAttackSfxVolumePercent = AttackSfxVolumePercent;
        ConfigChanged += OnConfigChanged;
    }

    [ConfigSection("AudioSettings")]
    [ConfigSlider(0.0, 300.0, 10.0, Format = "{0:0}%")]
    [ConfigHoverTip]
    public static float AttackSfxVolumePercent { get; set; } = 100.0f;

    [ConfigSection("EventSettings")]
    [ConfigHoverTip]
    public static bool ForceClassAltarFirstEvent { get; set; }

    public override void SetupConfigUI(Control optionContainer)
    {
        _lastAttackSfxVolumePercent = AttackSfxVolumePercent;
        base.SetupConfigUI(optionContainer);
    }

    private static void OnConfigChanged(object? sender, EventArgs e)
    {
        if (Mathf.IsEqualApprox(_lastAttackSfxVolumePercent, AttackSfxVolumePercent))
            return;

        _lastAttackSfxVolumePercent = AttackSfxVolumePercent;
        _previewToken?.Cancel();
        _previewToken?.Dispose();

        _previewToken = new CancellationTokenSource();
        PlayPreviewAfterDelay(_previewToken);
    }

    private static async void PlayPreviewAfterDelay(CancellationTokenSource tokenSource)
    {
        try
        {
            await Task.Delay(PreviewDelayMs, tokenSource.Token);
            StS2ArisMain.PlayAttackSfx("Aris_laser.mp3".SfxPath());
        }
        catch (OperationCanceledException)
        {
        }
    }
}
