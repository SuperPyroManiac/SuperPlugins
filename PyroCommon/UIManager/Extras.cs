using System.Drawing;
using PyroCommon.PyroFunctions;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace PyroCommon.UIManager;

internal class Extras
{
    internal static UIMenuItem SuperCallouts()
    {
        var ver = !Main.UsingSc ? "Not Installed!" : Main.InstalledPyroPlugins["SuperCallouts"];
        var menuItem = new UIMenuItem("SuperCallouts");
        menuItem.Skipped = true;
        menuItem.RightLabel = ver;
        if (!Main.UsingSc)
            return menuItem;
        if (VersionChecker.OutdatedPyroPlugins.ContainsKey("SuperCallouts"))
        {
            menuItem.ForeColor = Color.Red;
            menuItem.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            menuItem.LeftBadgeInfo.Color = Color.Yellow;
            return menuItem;
        }
        menuItem.ForeColor = Color.Green;
        menuItem.LeftBadge = UIMenuItem.BadgeStyle.Tick;
        return menuItem;
    }

    internal static UIMenuItem SuperEvents()
    {
        var ver = !Main.UsingSe ? "Not Installed!" : Main.InstalledPyroPlugins["SuperEvents"];
        var menuItem = new UIMenuItem("SuperEvents");
        menuItem.Skipped = true;
        menuItem.RightLabel = ver;
        if (!Main.UsingSe)
            return menuItem;
        if (VersionChecker.OutdatedPyroPlugins.ContainsKey("SuperEvents"))
        {
            menuItem.ForeColor = Color.Red;
            menuItem.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            menuItem.LeftBadgeInfo.Color = Color.Yellow;
            return menuItem;
        }
        menuItem.ForeColor = Color.Green;
        menuItem.LeftBadge = UIMenuItem.BadgeStyle.Tick;
        return menuItem;
    }

    internal static UIMenuItem DeadlyWeapons()
    {
        var ver = !Main.UsingDw ? "Not Installed!" : Main.InstalledPyroPlugins["DeadlyWeapons"];
        var menuItem = new UIMenuItem("DeadlyWeapons");
        menuItem.Skipped = true;
        menuItem.RightLabel = ver;
        if (!Main.UsingDw)
            return menuItem;
        if (VersionChecker.OutdatedPyroPlugins.ContainsKey("DeadlyWeapons"))
        {
            menuItem.ForeColor = Color.Red;
            menuItem.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            menuItem.LeftBadgeInfo.Color = Color.Yellow;
            return menuItem;
        }
        menuItem.ForeColor = Color.Green;
        menuItem.LeftBadge = UIMenuItem.BadgeStyle.Tick;
        return menuItem;
    }

    internal static string CenterText(UIMenu menu, string text)
    {
        var totalChars = (int)(menu.AdjustedWidth * 255);
        var padding = (totalChars - text.Length) / 2;
        var space = new string(' ', padding);
        var cText = space + text;
        return cText;
    }

    internal static UIMenuItem UiSeparator(string text)
    {
        var sep = new UIMenuItem(text);
        sep.Skipped = true;
        sep.BackColor = Color.FromArgb(220, 0, 0, 15);
        sep.ForeColor = Color.DarkGoldenrod;
        return sep;
    }
}
