using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Triangles.Filters;
using Triangles.Models;

namespace Triangles.Controllers;

[Route("triangle")]
[ValidTriangles]
public class TriangleController : Controller
{
    /// <summary>/triangle/Info?side1=10&amp;side2=20&amp;side3=25</summary>
    [HttpGet("Info")]
    public string Info([FromQuery] Triangle triangle) => triangle.Describe();

    /// <summary>/triangle/Area?side1=10&amp;side2=20&amp;side3=25</summary>
    [HttpGet("Area")]
    public string Area([FromQuery] Triangle triangle) =>
        triangle.Area.ToString("F4", CultureInfo.InvariantCulture);

    /// <summary>/triangle/Perimeter?side1=10&amp;side2=20&amp;side3=25</summary>
    [HttpGet("Perimeter")]
    public double Perimeter([FromQuery] Triangle triangle) => triangle.Perimeter;

    [HttpGet("IsRightAngled")]
    public bool IsRightAngled([FromQuery] Triangle triangle) => triangle.IsRightAngled();

    [HttpGet("IsEquilateral")]
    public bool IsEquilateral([FromQuery] Triangle triangle) => triangle.IsEquilateral();

    [HttpGet("IsIsosceles")]
    public bool IsIsosceles([FromQuery] Triangle triangle) => triangle.IsIsosceles();

    /// <summary>/triangle/AreCongruent?tr1.side1=10&amp;tr1.side2=20&amp;tr1.side3=15&amp;tr2.side1=15&amp;...</summary>
    [HttpGet("AreCongruent")]
    public bool AreCongruent([FromQuery] Triangle tr1, [FromQuery] Triangle tr2) => tr1.IsCongruentTo(tr2);

    [HttpGet("AreSimilar")]
    public bool AreSimilar([FromQuery] Triangle tr1, [FromQuery] Triangle tr2) => tr1.IsSimilarTo(tr2);

    /// <summary>/triangle/GreatestByPerimeter?tr[0].side1=10&amp;tr[0].side2=20&amp;tr[0].side3=12&amp;tr[1].side1=...</summary>
    [HttpGet("GreatestByPerimeter")]
    [HttpGet("GreatesByPerimeter")]
    public string InfoGreatestPerimeter([FromQuery] Triangle[] tr) =>
        tr.MaxBy(t => t.Perimeter)!.Describe();

    [HttpGet("GreatestByArea")]
    public string InfoGreatestArea([FromQuery] Triangle[] tr) =>
        tr.MaxBy(t => t.Area)!.Describe();

    /// <summary>
    /// Lists the 1-based index pairs "(i, j)", i '<' j, of triangles that are not similar to each other.
    /// </summary>
    [HttpGet("PairwiseNonSimilar")]
    public string NumbersPairwiseNotSimilar([FromQuery] Triangle[] tr)
    {
        var pairs =
            from i in Enumerable.Range(0, tr.Length)
            from j in Enumerable.Range(i + 1, tr.Length - i - 1)
            where !tr[i].IsSimilarTo(tr[j])
            select $"({i + 1}, {j + 1})";

        return string.Join(Environment.NewLine, pairs);
    }
}
