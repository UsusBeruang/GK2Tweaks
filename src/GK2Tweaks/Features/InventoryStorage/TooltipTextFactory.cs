using HarmonyLib;
using LazyBearTechnology;
using TMPro;
using UnityEngine;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal static class TooltipTextFactory
    {
        private static TextStyle _bodyStyle;
        private static TextStyle _accentStyle;

        internal static LazyWidgetDataBase CreateCenteredAccentText(string text)
        {
            TextStyle body = ResolveStyle("headerTextStyle", ref _bodyStyle);
            TextStyle accent = ResolveStyle("headerBoldTextStyleGold", ref _accentStyle);

            if (body == null || accent == null)
                return null;

            string styled = accent.ApplyStyleToString(text, false, true);
            return new UITooltipTextWidgetData(
                styled,
                TextAlignmentOptions.Center,
                body);
        }

        private static TextStyle ResolveStyle(string fieldName, ref TextStyle cache)
        {
            if (cache != null)
                return cache;

            UITooltip tooltip = Traverse.Create(typeof(UITooltip))
                .Field("instance")
                .GetValue<UITooltip>();

            if (tooltip == null)
                return null;

            cache = Traverse.Create(tooltip)
                .Field(fieldName)
                .GetValue<TextStyle>();

            return cache;
        }
    }
}
