using System;
using System.Drawing;
using RAGENativeUI;

namespace PyroCommon.UIManager;

public class Style
{
    public static void ApplyStyle(MenuPool pool, bool center)
    {
        foreach (var men in pool)
        {
            men.SetBannerType(Color.FromArgb(240, 0, 0, 15));
            men.TitleStyle = men.TitleStyle with
            {
                Color = Color.DarkGoldenrod,
                Font = TextFont.ChaletComprimeCologne,
                DropShadow = true,
                Outline = true,
            };
            men.MouseControlsEnabled = false;
            men.AllowCameraMovement = true;
            men.MaxItemsOnScreen = 20;
            if (!center)
                return;
            var screenWidth = UIMenu.GetActualScreenResolution().Width;
            var menuWidth = men.Width * screenWidth + (men.WidthOffset != 0 ? men.WidthOffset : 0);
            var cnt = Math.Min(men.MenuItems.Count, 20);
            men.Offset = new Point((int)((screenWidth - menuWidth) / 2f), (int)((1080f - (cnt * 38f + 107f + 20f)) / 2f));
        }
    }
}
