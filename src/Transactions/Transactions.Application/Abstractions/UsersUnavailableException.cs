namespace Transactions.Application.Abstractions;

public class UsersUnavailableException : Exception
{
  public UsersUnavailableException(string message, Exception? inner = null) : base(message, inner) { }
}
