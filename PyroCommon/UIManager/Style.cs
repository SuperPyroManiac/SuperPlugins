using System.Drawing;
using Rage;
using RAGENativeUI;

namespace PyroCommon.UIManager;

public class Style
{
    public static void ApplyStyle(MenuPool pool, bool center)
    {
        foreach ( var men in pool )
        {
            men.SetBannerType(Color.FromArgb(240, 0, 0, 15));
            men.TitleStyle = men.TitleStyle with
            {
                Color = Color.DarkGoldenrod,
                Font = TextFont.ChaletComprimeCologne,
                DropShadow = true,
                Outline = true
            };
            men.MouseControlsEnabled = false;
            men.AllowCameraMovement = true;
            men.MaxItemsOnScreen = 20;
            if ( !center ) return;
            var cnt = men.MenuItems.Count;
            if ( cnt > 20 ) cnt = 20;
            men.Offset = new Point((int)((Game.Resolution.Width / 2f) - (men.Width / 2f)), (int)((Game.Resolution.Height / 2f) - ((cnt * 38f) + 107f + 20f) / 2));
            //var red = UIMenu.GetActualScreenResolution();
        }
    }
}