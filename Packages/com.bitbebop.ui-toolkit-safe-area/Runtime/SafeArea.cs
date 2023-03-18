using UnityEngine;
using UnityEngine.UIElements;

namespace Bitbebop
{
    /// <summary>
    /// SafeArea for UI Toolkit.
    /// </summary>
    public class SafeArea : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SafeArea, UxmlTraits> {}

        public SafeArea()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var safeArea = Screen.safeArea;

            var leftTop = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            var rightBottom = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            style.marginLeft = leftTop.x;
            style.marginTop = leftTop.y;
            style.marginRight = rightBottom.x;
            style.marginBottom = rightBottom.y;
        }
    }
}