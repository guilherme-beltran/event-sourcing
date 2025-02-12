namespace EventSourcing.BankAccounts;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
}

public class BankTransaction
{
    public DateTime TimeStamp { get; }
    public TransactionType Type { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string Description { get; }

    private BankTransaction(DateTime timeStamp, TransactionType type, decimal amount, string currency, string description)
    {
        TimeStamp = timeStamp;
        Type = type;
        Amount = amount;
        Currency = currency;
        Description = description;
    }

    public static BankTransaction Generate (DateTime timeStamp, TransactionType type, decimal amount, string currency, string description)
    {
        BankTransaction transaction = new(timeStamp, type, amount, currency, description);
        return transaction;
    }

    public override string ToString()
    {
        string sign = Type == TransactionType.Deposit ? "+" : "-";
        return $"{TimeStamp:yyyy-MM-dd HH:mm:ss} | {Type,-15} | {sign}{Amount:C} {Currency} | {Description}";
    }
}

