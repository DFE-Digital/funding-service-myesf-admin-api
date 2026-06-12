using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "StyleCop.CSharp.LayoutRules",
    "SA1515:Single-line comment should be preceded by blank line",
    Justification = "unnecessary in test assembly",
    Scope = "module")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Test classes do not require documentation",
    Scope = "module")]