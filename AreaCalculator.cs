using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidPrinciplesOOP.Luigi
{
    internal interface IShape
    {

    }

    internal interface IOpenShape : IShape
    {
        double GetLength();
    }

    internal interface IClosedShape : IShape
    {
        double GetArea();
    }

    internal interface IColorable : IFillColorable, IStrokeColorable
    {
    }

    internal interface IFillColorable
    {
        void SetFillColor(Color color);
    }

    internal interface IStrokeColorable
    {
        void SetStrokeColor(Color color);
    }

    internal abstract class ClosedShape : IClosedShape, IColorable
    {
        public abstract double GetArea();

        public Color FillColor { get; set; }
        public Color StrokeColor { get; set; }

        public int SidesCount { get; set; }
        public override string ToString()
        {
            return $"{GetType()}: {SidesCount} sides";
        }

        public void SetFillColor(Color color)
        {
            FillColor = color;
        }

        public void SetStrokeColor(Color color)
        {
            StrokeColor = color;
        }
    }

    internal class Circle : ClosedShape
    {
        public Circle(double radius)
        {
            Radius = radius;
        }
        public double Radius { get; set; }

        public override double GetArea()
        {
            return Radius * Radius * Math.PI;
        }
    }
    internal class Rectangle : ClosedShape
    {
        public Rectangle(double height, double width)
        {
            Height = height;
            Width = width;
        }

        public double Height { get; set; }
        public double Width { get; set; }

        public override double GetArea()
        {
            return Width * Height;
        }
    }
    internal class Line : IOpenShape, IStrokeColorable
    {
        public Color StrokeColor { get; set; }

        public Point Start { get; set; }
        public Point End { get; set; }

        public double GetArea()
        {
            throw new InvalidOperationException();
        }

        public double GetLength()
        {
            // teorema di pitagora
            return 0;
        }

        public void SetStrokeColor(Color color)
        {
            StrokeColor = color;
        }
    }

    internal class AreaCalculator
    {
        private IClosedShape[] _shapes;
        private IShapesLoader _loader;
        private IAreaFormatter _areaFormatter;

        public AreaCalculator(IShapesLoader shapesLoader, IAreaFormatter areaFormatter)
        {
            _loader = shapesLoader;
            _areaFormatter = areaFormatter;

            _shapes = _loader.LoadShapes()
                .Where(e => e.GetType().IsAssignableTo(typeof(IClosedShape)))
                .Cast<IClosedShape>()
                .ToArray();
        }

        public double GetArea()
        {
            double area = 0;
            foreach (var item in _shapes)
            {
                area += item.GetArea();
            }
            return area;
        }

        public string PrintAreaInfo()
        {
            return _areaFormatter.PrintAreaInfo(GetArea());
        }
    }

    internal interface IShapesLoader
    {
        IShape[] LoadShapes();
    }
    internal class CsvShapesLoader : IShapesLoader
    {
        private string _filePath;

        public CsvShapesLoader(string filePath)
        {
            _filePath = filePath;
        }

        private Func<string, IShape> _shapeFactory => (line) =>
        {
            var values = line.Split(',');
            int type = int.Parse(values[0]);
            return type switch
            {
                1 => new Rectangle(double.Parse(values[1]), double.Parse(values[2])),
                2 => new Circle(double.Parse(values[1])),
                _ => throw new ArgumentOutOfRangeException()
            };
        };

        private IShape[] loadShapes(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return lines.Select(line =>
            {
                return _shapeFactory(line);
            }).ToArray();
        }

        public IShape[] LoadShapes()
        {
            return loadShapes(_filePath);
        }
    }

    internal interface IAreaFormatter
    {
        string PrintAreaInfo(double area);
    }

    internal class HtmlAreaFormatter : IAreaFormatter
    {
        public string PrintAreaInfo(double area)
        {
            return $"<div>Area: {area}</div>";
        }
    }

    internal class JsonAreaFormatter : IAreaFormatter
    {
        public string PrintAreaInfo(double area)
        {
            return $"{{ \"area\": {area} }}";
        }
    }
}
