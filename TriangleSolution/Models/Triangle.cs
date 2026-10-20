using System.Globalization;

namespace Triangles.Models;

/// <summary>
/// A triangle given by its three side lengths. Bound from the query string
/// (<c>side1</c>, <c>side2</c>, <c>side3</c>
/// </summary>
public sealed record Triangle(double Side1, double Side2, double Side3)
{
    private const double RelativeTolerance = 1e-5;

    /// <summary>Sides in ascending order</summary>
    public Triangle Ordered()
    {
        Span<double> sides = [Side1, Side2, Side3];
        sides.Sort();
        return new Triangle(sides[0], sides[1], sides[2]);
    }

    public double Perimeter
    {
        get
        {
            var t = Ordered();
            return t.Side1 + t.Side2 + t.Side3;
        }
    }

    /// <summary>Heron's formula</summary>
    public double Area
    {
        get
        {
            var s = Perimeter / 2;
            return Math.Sqrt(Math.Max(0, s * (s - Side1) * (s - Side2) * (s - Side3)));
        }
    }
    
    public string? GetValidationError()
    {
        if (!double.IsFinite(Side1) || !double.IsFinite(Side2) || !double.IsFinite(Side3))
            return "Sides must be finite numbers.";

        var t = Ordered();
        if (t.Side1 <= 0)
            return "Sides must be positive.";

        if (t.Side1 + t.Side2 <= t.Side3)
            return "The longest side must be shorter than the sum of the other two.";

        return null;
    }

    public bool IsRightAngled()
    {
        var t = Ordered();
        return AreEqual(double.Hypot(t.Side1, t.Side2), t.Side3);
    }

    public bool IsEquilateral()
    {
        var t = Ordered();
        return AreEqual(t.Side1, t.Side2) && AreEqual(t.Side1, t.Side3);
    }
    
    public bool IsIsosceles()
    {
        var t = Ordered();
        return AreEqual(t.Side1, t.Side2) || AreEqual(t.Side2, t.Side3) || AreEqual(t.Side1, t.Side3);
    }

    public bool IsCongruentTo(Triangle other)
    {
        var a = Ordered();
        var b = other.Ordered();
        return AreEqual(a.Side1, b.Side1) && AreEqual(a.Side2, b.Side2) && AreEqual(a.Side3, b.Side3);
    }

    public bool IsSimilarTo(Triangle other)
    {
        var a = Ordered().Reduced();
        var b = other.Ordered().Reduced();
        return AreEqual(a.Side1, b.Side1) && AreEqual(a.Side2, b.Side2) && AreEqual(a.Side3, b.Side3);
    }
    
    public Triangle Reduced()
    {
        var p = Perimeter;
        return new Triangle(Side1 / p, Side2 / p, Side3 / p);
    }
    
    public string Describe()
    {
        var sorted = Ordered();
        var reduced = sorted.Reduced();
        var nl = Environment.NewLine;
        var c = CultureInfo.InvariantCulture;

        return string.Create(c,
            $"Triangle:{nl}({sorted.Side1}, {sorted.Side2}, {sorted.Side3}){nl}" +
            $"Reduced:{nl}({reduced.Side1:F2}, {reduced.Side2:F2}, {reduced.Side3:F2}){nl}{nl}" +
            $"Area = {Area:F2}{nl}Perimeter = {Perimeter}");
    }

    private static bool AreEqual(double first, double second) =>
        first == second || Math.Abs(first - second) < RelativeTolerance * Math.Abs(first);
}
