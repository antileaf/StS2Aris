namespace StS2Aris.StS2ArisCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/{path}";
    }

    public static string CardImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/card_portraits/{path}";
    }

    public static string PowerImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/powers/{path}";
    }

    public static string BigPowerImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/powers/big/{path}";
    }

    public static string RelicImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/relics/{path}";
    }

    public static string BigRelicImagePath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/relics/big/{path}";
    }

    public static string CharacterUiPath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/images/charui/{path}";
    }
    
    
    public static string ScencesPath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/scenes/{path}";
    }
    
    public static string SfxPath(this string path)
    {
        return $"res://{StS2ArisMain.ModId}/audio/{path}";
    }
}