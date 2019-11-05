namespace cAlgo.API.Extensions.Controls
{
    public class ChartObjectTooltip : CustomControl
    {
        #region Fields

        private readonly Border _border;

        private readonly Chart _chart;

        private readonly ChartObject _chartObject;

        private double _lastMouseX, _lastMouseY;

        #endregion Fields

        public ChartObjectTooltip(Chart chart, ChartObject chartObject)
        {
            _chart = chart;
            _chartObject = chartObject;

            _border = new Border
            {
                BackgroundColor = "#3F3F3F",
                BorderColor = "#969696",
                BorderThickness = 1,
                CornerRadius = 5
            };

            AddChild(_border);

            _chart.ObjectHoverChanged += Chart_ObjectHoverChanged;
            _chart.MouseMove += Chart_MouseMove;
        }

        #region Properties

        public Color BackgroundColor
        {
            get
            {
                return _border.BackgroundColor;
            }
            set
            {
                _border.BackgroundColor = value;
            }
        }

        public Color BorderColor
        {
            get
            {
                return _border.BorderColor;
            }
            set
            {
                _border.BorderColor = value;
            }
        }

        public Thickness BorderThickness
        {
            get
            {
                return _border.BorderThickness;
            }
            set
            {
                _border.BorderThickness = value;
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return _border.CornerRadius;
            }
            set
            {
                _border.CornerRadius = value;
            }
        }

        public ControlBase Content
        {
            get
            {
                return _border.Child;
            }
            set
            {
                _border.Child = value;
            }
        }

        #endregion Properties

        #region Methods

        private void Chart_ObjectHoverChanged(ChartObjectHoverChangedEventArgs obj)
        {
            if (obj.IsObjectHovered && obj.ChartObject == _chartObject)
            {
                UpdateCoorinates();

                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
        }

        private void Chart_MouseMove(ChartMouseEventArgs obj)
        {
            _lastMouseX = obj.MouseX;
            _lastMouseY = obj.MouseY;

            if (IsVisible)
            {
                UpdateCoorinates();
            }
        }

        private void UpdateCoorinates()
        {
            var extraDelta = 10;
            var width = Width;
            var height = Height;
            var left = _chart.Width - _lastMouseX > width + extraDelta ? _lastMouseX + extraDelta : _lastMouseX - width - extraDelta;
            var right = _chart.Height - _lastMouseY > height + extraDelta ? _lastMouseY + extraDelta : _lastMouseY - height - extraDelta;

            Margin = new Thickness(left, right, 0, 0);
        }

        #endregion Methods
    }
}