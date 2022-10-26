namespace Crone;
public abstract class CoreDataCommandBase<TCommand, TResult> : CoreComponent
    where TCommand : ICoreDataCommand
{
    public ICoreDataProvider Provider { get; }
    public abstract TCommand Command { get; }

    public CoreDataCommandBase(ICoreDataProvider provider) => Provider = provider;

    public TResult Execute() => Execute(Command);
    protected abstract TResult Execute(ICoreDataCommand command);
}
