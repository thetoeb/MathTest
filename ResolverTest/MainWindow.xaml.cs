using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExpressionResolver;
using ExpressionResolver.Exceptions;
using ExpressionResolver.Interface;

namespace ResolverTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Resolver _resolver;
        private decimal[] _values;
        private int _maxValues = 200;
        private int _startValue = -100;
        private decimal _step = (decimal) 0.2;

        private double _xStep = 20;
        private double _yStep = 20;


        private Path _graph;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawAxis(Canvas);
        }

        private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TrySolve(Input.Text);
        }

        private void TrySolve(string t)
        {
            try
            {
                _resolver = new Resolver(t);
                if (!_resolver.Variables.ContainsKey("x"))
                    return;

                if (_values == null || _values.Length != _maxValues)
                    _values = new decimal[_maxValues];

                for (int i = 0; i < _maxValues; i++)
                {
                    var x = (_startValue + i) * _step;
                    _resolver.Variables["x"] = x;
                    try
                    {
                        _values[i] = _resolver.Resolve();
                    }
                    catch (DivideByZeroException)
                    {
                        _values[i] = 0;
                    }
                }

                DrawGraph(Canvas);
            }
            catch (InvalidExpressionException)
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void DrawGraph(Canvas c)
        {
            if(_graph != null)
                c.Children.Remove(_graph);

            var width = c.ActualWidth;
            var height = c.ActualHeight;

            var grp = new GeometryGroup();
            var lastPoint = new Point();
            for (var i = 0; i < _maxValues; i++)
            {
                var x = (double) ((_startValue + i) * _step);
                var y = (double) _values[i];
                var point = new Point(width/2 + x * _xStep, height/2 - y * _yStep);
                if (i > 0)
                {
                    grp.Children.Add(new LineGeometry(lastPoint, point));
                }
                lastPoint = point;
            }

            _graph = AddGeometrytoCanvas(grp, c, 1, Brushes.Crimson);
            c.InvalidateVisual();
            c.UpdateLayout();
        }

        private void DrawAxis(Canvas c)
        {
            var width = c.ActualWidth;
            var height = c.ActualHeight;
            var lines = 10;

            var xaxis = new GeometryGroup();
            xaxis.Children.Add(new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)));

            for (double x = _xStep; x < width / 2; x += _xStep)
            {
                xaxis.Children.Add(new LineGeometry(new Point(width / 2 + x, height / 2 - lines / 2), new Point(width / 2 + x, height / 2 + lines / 2)));
                xaxis.Children.Add(new LineGeometry(new Point(width / 2 - x, height / 2 - lines / 2), new Point(width / 2 - x, height / 2 + lines / 2)));
            }

            AddGeometrytoCanvas(xaxis, c, 1, Brushes.Black);
           
            var yaxis = new GeometryGroup();
            yaxis.Children.Add(new LineGeometry(new Point(width / 2, 0), new Point(width / 2, height)));

            for (double y = _yStep; y < height / 2; y += _yStep)
            {
                yaxis.Children.Add(new LineGeometry(new Point(width / 2 - lines / 2, height / 2 + y), new Point(width / 2 + lines / 2, height / 2 + y)));
                yaxis.Children.Add(new LineGeometry(new Point(width / 2 - lines / 2, height / 2 - y), new Point(width / 2 + lines / 2, height / 2 - y)));
            }

            AddGeometrytoCanvas(yaxis, c, 1, Brushes.Black);
        }

        private Path AddGeometrytoCanvas(GeometryGroup grp, Canvas c, double strokeThickness, Brush stroke)
        {
            var path = new Path();
            path.StrokeThickness = strokeThickness;
            path.Stroke = stroke;
            path.Data = grp;
            c.Children.Add(path);
            return path;
        }


        private void Canvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.Children.Clear();
            DrawAxis(Canvas);
            if (_values != null)
            {
                DrawGraph(Canvas);
            }
        }
    }
}
