using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Triangles.Models;

namespace Triangles.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ValidTrianglesAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var error = FindError(context);
        if (error is null || context.Controller is not Controller controller)
            return;

        var view = controller.View("Error", new ErrorViewModel(error));
        view.StatusCode = StatusCodes.Status400BadRequest;
        context.Result = view;
    }

    private static string? FindError(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var messages = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                .Distinct();
            return string.Join(" ", messages);
        }

        foreach (var (name, value) in context.ActionArguments)
        {
            switch (value)
            {
                case Triangle triangle:
                    if (Check(name, triangle) is { } single)
                        return single;
                    break;

                case Triangle[] { Length: 0 }:
                    return "At least one triangle is required.";

                case Triangle[] triangles:
                    for (var i = 0; i < triangles.Length; i++)
                        if (Check($"{name}[{i}]", triangles[i]) is { } indexed)
                            return indexed;
                    break;

                case null:
                    return $"The '{name}' parameter is required.";
            }
        }

        return null;
    }

    private static string? Check(string name, Triangle triangle)
    {
        if (triangle.GetValidationError() is not { } reason)
            return null;

        var c = CultureInfo.InvariantCulture;
        return string.Create(c,
            $"{name} ({triangle.Side1}, {triangle.Side2}, {triangle.Side3}) is not a valid triangle. {reason}");
    }
}
