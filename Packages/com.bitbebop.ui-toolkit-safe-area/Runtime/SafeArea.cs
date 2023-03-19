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
            style.flexGrow = 1;
            style.flexShrink = 0;
            
            _contentContainer = new VisualElement();
            _contentContainer.name = "safe-area-content-container";
            _contentContainer.style.flexGrow = 1;
            _contentContainer.style.flexShrink = 0;
            hierarchy.Add(_contentContainer);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // As RuntimePanelUtils is not available in UIBuilder, the handling is wrapped in a try catch statement
            // to avoid InvalidCastExceptions when working in UIBuilder.
            try
            {
                // Convert screen safe area to panel space.
                var safeArea = Screen.safeArea;
                var leftTopSafeArea = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
                var rightBottomSafeArea = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

                // Convert and calculate the right bottom margins.
                var rightBottomScreen = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width, Screen.height));
                var rightBottomMargin = rightBottomScreen - new Vector2(layout.xMax, layout.yMax);

                if (CollapseMargin)
                {
                    _contentContainer.style.marginTop = Mathf.Max(layout.y, leftTopSafeArea.y) - layout.y;
                    _contentContainer.style.marginLeft = (Mathf.Max(layout.x, leftTopSafeArea.x)) - layout.x;
                    _contentContainer.style.marginBottom = Mathf.Max(rightBottomMargin.y, rightBottomSafeArea.y) -rightBottomMargin.y;
                    _contentContainer.style.marginRight = (Mathf.Max(rightBottomMargin.x, rightBottomSafeArea.x)) - rightBottomMargin.x;
                }
                else
                {
                    _contentContainer.style.marginTop = layout.y + leftTopSafeArea.y;
                    _contentContainer.style.marginLeft = (layout.x + leftTopSafeArea.x);
                    _contentContainer.style.marginBottom = rightBottomMargin.y + rightBottomSafeArea.y;
                    _contentContainer.style.marginRight = rightBottomMargin.x + rightBottomSafeArea.x;
                }
            }
            catch (System.InvalidCastException) {}
        }
    }
}