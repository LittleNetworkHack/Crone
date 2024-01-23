namespace Crone;
public sealed class CoreTransactionScope : IDisposable
{
	private static readonly AsyncLocal<CoreTransactionScope> currentScope = new AsyncLocal<CoreTransactionScope>();

	private readonly Stack<DbConnection> connectionStack;
	private readonly Stack<DbTransaction> transactionStack;

	private DbConnection currentConnection;
	private DbTransaction currentTransaction;

	private bool Completed { get; set; }

	public CoreTransactionScope()
	{
		connectionStack = new Stack<DbConnection>();
		transactionStack = new Stack<DbTransaction>();

		currentScope.Value = this;
	}

	public void Commit()
	{
		if (Completed)
		{
			throw new InvalidOperationException("Database transaction already completed!");
		}

		var transaction = GetCurrentTransaction();
		transaction?.Commit();
		transaction?.Dispose();
		//transactionStack.Pop();

		//if (transactionStack.Count == 0)
		//{
		//	currentScope.Value = null;
		//}
		currentScope.Value = null;
		Completed = true;
	}

	public void Rollback()
	{
		if (Completed)
		{
			throw new InvalidOperationException("Database transaction already completed!");
		}

		var transaction = GetCurrentTransaction();
		transaction?.Rollback();
		transaction?.Dispose();
		//transactionStack.Pop();

		//if (transactionStack.Count == 0)
		//{
		//	currentScope.Value = null;
		//}
		currentScope.Value = null;
		Completed = true;
	}

	public void Dispose()
	{
		if (Completed)
		{
			return;
		}

		Rollback();
	}

	public static CoreTransactionScope Current => currentScope.Value;

	public static bool IsInsideTransaction()
	{
		return Current != null;
	}

	public static DbConnection GetCurrentConnection()
	{
		//var current = Current;
		//if (current == null || current.connectionStack.Count == 0)
		//{
		//	return null;
		//}
		//return current.connectionStack.Peek();
		return Current?.currentConnection;
	}

	public static DbTransaction GetCurrentTransaction()
	{
		//var current = Current;
		//if (current == null || current.transactionStack.Count == 0)
		//{
		//	return null;
		//}

		//return current.transactionStack.Peek();
		return Current?.currentTransaction;
	}

	public static void SetCurrentConnection(DbConnection connection)
	{
		//Current.connectionStack.Push(connection);
		Current.currentConnection = connection;
	}

	public static void SetCurrentTransaction(DbTransaction transaction)
	{
		//Current.transactionStack.Push(transaction);
		Current.currentTransaction = transaction;
	}
}