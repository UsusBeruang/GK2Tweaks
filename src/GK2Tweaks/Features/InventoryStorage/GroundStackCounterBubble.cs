using System.Collections.Generic;
using LazyBearTechnology;
using UnityEngine;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class GroundStackCounterBubble : IBubbleDrawable
    {
        private readonly DropView _view;
        private readonly List<LazyWidgetDataBase> _widgets = new List<LazyWidgetDataBase>();

        internal GroundStackCounterBubble(DropView view)
        {
            _view = view;
        }

        public SGuid BubbleDrawableUniqueId => _view.Data.UniqueId;

        public List<LazyWidgetDataBase> BubbleDrawableWidgets => _widgets;

        public Vector3 BubbleDrawablePosition =>
            _view.transform.position + Vector3.up * 0.5f;

        internal void Refresh()
        {
            _widgets.Clear();

            int count = _view.Data?.Count ?? 0;
            if (count <= 1)
                return;

            LazyWidgetDataBase widget =
                TooltipTextFactory.CreateCenteredAccentText($"x{count}");

            if (widget != null)
                _widgets.Add(widget);
        }
    }
}
