using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bitbebop
{
    /// <summary>
    /// SafeArea Container for UI Toolkit.
    /// </summary>
    public class SafeArea : VisualElement
    {
        private struct Offset
        {
            public float Left, Right, Top, Bottom;

            public override string ToString()
            {
                return $"l: {Left}, r: {Right}, t:{Top}, b: {Bottom}";
            }
        }

        public new class UxmlFactory : UxmlFactory<SafeArea, UxmlTraits> {}

        public bool CollapseMargin { get; set; }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlBoolAttributeDescription _collapseMarginAttr = new() { name = "collapse-margin", defaultValue = true };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as SafeArea;

                ate.CollapseMargin = _collapseMarginAttr.GetValueFromBag(bag, cc);
            }
        }

        private VisualElement _contentContainer;
        public override VisualElement contentContainer {
            get => _contentContainer;
        }

        public SafeArea()
        {
            // By using absolute position instead of flex to fill the full screen, SafeArea containers can be stacked.
            style.position = Position.Absolute;
            style.top = 0;
            style.bottom = 0;
            style.left = 0;
            style.right = 0;
            
            _contentContainer = new VisualElement();
            _contentContainer.name = "safe-area-content-container";
            _contentContainer.style.flexGrow = 1;
            _contentContainer.style.flexShrink = 0;
            hierarchy.Add(_contentContainer);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // As RuntimePanelUtils is not available in UIBuilder,
            // the handling is wrapped in a try/catch to avoid InvalidCastExceptions when working in UIBuilder.
            try
            {
                var safeArea = GetSafeAreaOffset();
                var margin = GetMarginOffset();

                if (CollapseMargin)
                {
                    _contentContainer.style.marginLeft = Mathf.Max(margin.Left, safeArea.Left) - margin.Left;
                    _contentContainer.style.marginRight = Mathf.Max(margin.Right, safeArea.Right) - margin.Right;
                    _contentContainer.style.marginTop = Mathf.Max(margin.Top, safeArea.Top) - margin.Top;
                    _contentContainer.style.marginBottom = Mathf.Max(margin.Bottom, safeArea.Bottom) - margin.Bottom;
                }
                else
                {
                    _contentContainer.style.marginLeft = (margin.Left + safeArea.Left);
                    _contentContainer.style.marginRight = margin.Right + safeArea.Right;
                    _contentContainer.style.marginTop = margin.Top + safeArea.Top;
                    _contentContainer.style.marginBottom = margin.Bottom + safeArea.Bottom;
                }
            }
            catch (System.InvalidCastException) {}
        }

        // Convert screen safe area to panel space and get the offset values from the panel edges.
        private Offset GetSafeAreaOffset()
        {
            var safeArea = Screen.safeArea;
            var leftTop = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            var rightBottom = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            return new Offset()
            {
                Left = leftTop.x,
                Right = rightBottom.x,
                Top = leftTop.y,
                Bottom = rightBottom.y
            };
        }

        // Get the resolved margins from the inline style.
        private Offset GetMarginOffset()
        {
            return new Offset()
            {
                Left = resolvedStyle.marginLeft,
                Right = resolvedStyle.marginRight,
                Top = resolvedStyle.marginTop,
                Bottom = resolvedStyle.marginBottom,
            };
        }
    }
}