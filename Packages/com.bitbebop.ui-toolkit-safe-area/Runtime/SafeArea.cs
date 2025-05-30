#if UNITY_2023_2_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;

namespace Bitbebop
{
    /// <summary>
    /// SafeArea Container for UI Toolkit.
    /// </summary>
    [UxmlElement]
    public partial class SafeArea : VisualElement
    {
        [UxmlAttribute("collapse-margins")]
        private bool _collapseMargins = true;
        [UxmlAttribute("exclude-left")]
        private bool _excludeLeft;
        [UxmlAttribute("exclude-right")]
        private bool _excludeRight;
        [UxmlAttribute("exclude-top")]
        private bool _excludeTop;
        [UxmlAttribute("exclude-bottom")]
        private bool _excludeBottom;
        [UxmlAttribute("exclude-tvos")]
        private bool _excludeTvos;
        [UxmlAttribute("force-orientation-check")]
        private bool _forceOrientationCheck; 

        private struct Offset
        {
            public float Left, Right, Top, Bottom;

            public override string ToString()
            {
                return $"l: {Left}, r: {Right}, t:{Top}, b: {Bottom}";
            }
        }
        
        private VisualElement _contentContainer;
        public override VisualElement contentContainer {
            get => _contentContainer;
        }

        // Handle polling for orientation changes.
        private ScreenOrientation _screenOrientation;
        private IVisualElementScheduledItem _orientationPoller;
        
        public SafeArea()
        {
            // By using absolute position instead of flex to fill the full screen, SafeArea containers can be stacked.
            style.position = Position.Absolute;
            style.top = 0;
            style.bottom = 0;
            style.left = 0;
            style.right = 0;
            pickingMode = PickingMode.Ignore;
            
            _contentContainer = new VisualElement();
            _contentContainer.name = "safe-area-content-container";
            _contentContainer.pickingMode = PickingMode.Ignore;
            _contentContainer.style.flexGrow = 1;
            _contentContainer.style.flexShrink = 0;
            hierarchy.Add(_contentContainer);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            // Setup a scheduled item that can poll for orientation changes. Starts in a paused state.
            _orientationPoller = schedule.Execute(() =>
            {
                // Don't run if not in a panel (prevents errors on detached elements)
                if (panel == null) return; 

                // Check if there is a portrait up/down (3) or landscape left/right (7) change.
                if (((int) _screenOrientation ^ (int) Screen.orientation) is 3 or 7)
                    OnGeometryChanged(null);

                _screenOrientation = Screen.orientation;
            }).Every(250); // Poll at reasonable fixed interval (4 times a second).
            _orientationPoller.Pause();
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // Start the orientation poller if it is enabled
            if (_forceOrientationCheck)
            {
                _screenOrientation = Screen.orientation;
                _orientationPoller?.Resume();
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // Always stop the polling when detached from a panel.
            _orientationPoller?.Pause();
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // As RuntimePanelUtils is not available in UIBuilder,
            // the handling is wrapped in a try/catch to avoid InvalidCastExceptions when working in UIBuilder.
            try
            {
                var safeArea = GetSafeAreaOffset();
                var margin = GetMarginOffset();

                if (_collapseMargins)
                {
                    _contentContainer.style.marginLeft = Mathf.Max(margin.Left, safeArea.Left) - margin.Left;
                    _contentContainer.style.marginRight = Mathf.Max(margin.Right, safeArea.Right) - margin.Right;
                    _contentContainer.style.marginTop = Mathf.Max(margin.Top, safeArea.Top) - margin.Top;
                    _contentContainer.style.marginBottom = Mathf.Max(margin.Bottom, safeArea.Bottom) - margin.Bottom;
                }
                else
                {
                    _contentContainer.style.marginLeft = safeArea.Left;
                    _contentContainer.style.marginRight = safeArea.Right;
                    _contentContainer.style.marginTop = safeArea.Top;
                    _contentContainer.style.marginBottom = safeArea.Bottom;
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

#if UNITY_TVOS
            if (_excludeTvos)
                return new Offset { Left = 0, Right = 0, Top = 0, Bottom = 0 };
#endif

            // If the user has flagged an edge as excluded, set that edge to 0.
            return new Offset()
            {
                Left = _excludeLeft ? 0 : leftTop.x,
                Right = _excludeRight ? 0 : rightBottom.x,
                Top = _excludeTop ? 0 : leftTop.y,
                Bottom = _excludeBottom ? 0 : rightBottom.y
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
#endif