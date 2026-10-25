namespace Memorio.Modularity;

/// <summary>
/// Marks an EF Core migration that contracts the schema (drops, renames, narrows or makes required what
/// the running version may still use), which the expand/contract rule refuses otherwise (SD18, D-041).
/// A contract migration ships only in a release after the one whose code stopped using what it removes,
/// so the previous version keeps working while the new one starts and after a rollback.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ContractMigrationAttribute(string reason) : Attribute
{
    /// <summary>Why it is safe: which earlier release stopped using what this migration removes.</summary>
    public string Reason { get; } = reason;
}
