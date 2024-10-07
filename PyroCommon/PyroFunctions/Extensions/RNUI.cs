using System;
using PyroCommon.UIManager;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace PyroCommon.PyroFunctions.Extensions;

public static class RNUI
{
        private static T WithTextEditingBase<T>(T item, int maxLength, Func<string> strGetter, Action<string> resultCallback) where T : UIMenuItem
    {
        item = item ?? throw new ArgumentNullException(nameof(item));
        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException("Length cannot be negative", nameof(maxLength));
        }

        item.Activated += (m, s) =>
        {
            Manager.MainMenuPool.Draw();
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "", "", strGetter(), "", "", "", maxLength);
            int state;
            while ((state = NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>()) == 0)
            {
                GameFiber.Yield();
                Manager.MainMenuPool.Draw();
            }
            if (state == 1)
            {
                string str = NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
                resultCallback(str);
            }
        };
        return item;
    }
    /// <summary>
    /// Allows to edit a string by selecting the item. The current string is displayed in the item's <see cref="UIMenuItem.RightLabel"/>.
    /// </summary>
    /// <param name="getter">Gets the string to display to the user.</param>
    /// <param name="setter">Takes the string edited by the user.</param>
    /// <param name="maxLength">The maximum length of the string.</param>
    /// <param name="maxLengthInItem">
    /// The maximum length of the string when set to the <see cref="UIMenuItem.RightLabel"/> property.
    /// If the string length exceeds this value, the string is cut and "..." is appended.
    /// </param>
    public static UIMenuItem WithTextEditing(this UIMenuItem item, Func<string> getter, Action<string> setter, int maxLengthInItem = 16, int maxLength = 32)
    {
        getter = getter ?? throw new ArgumentNullException(nameof(getter));
        setter = setter ?? throw new ArgumentNullException(nameof(setter));

        WithTextEditingBase(item, maxLength,
            getter,
            str =>
            {
                TrimAndSetRightLabel(item, str, maxLengthInItem);
                setter(str);
            });

        TrimAndSetRightLabel(item, getter(), maxLengthInItem);
        return item;

        static void TrimAndSetRightLabel(UIMenuItem item, string str, int maxLength)
            => item.RightLabel = str.Length > maxLength ? (str.Substring(0, maxLength) + "...") : str;
    }
}