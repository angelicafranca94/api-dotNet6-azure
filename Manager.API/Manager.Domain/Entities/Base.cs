namespace Manager.Domain.Entities;

public abstract class Base
{
    public long Id { get; set; }

    internal List<string> _erros;

    public IReadOnlyCollection<string> Errors => _erros;

    public abstract bool Validate();
}
