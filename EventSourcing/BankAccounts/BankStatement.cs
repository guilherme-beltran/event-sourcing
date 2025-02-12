using EventSourcing.Events;

namespace EventSourcing.BankAccounts;

public class BankStatement : Entity
{
    public string AccountHolder { get; private set;}
    public string Currency { get; private set;}
    public DateTime StartDate { get; private set;}
    public DateTime EndDate { get; private set;}
    public decimal InitialBalance { get; private set; }
    public decimal FinalBalance { get; private set; }
    public List<BankTransaction> Transactions { get; }

    private BankStatement(string accountHolder, string currency, DateTime startDate, DateTime endDate, decimal initialBalance, decimal finalBalance, List<BankTransaction> transactions)
    {
        AccountHolder = accountHolder;
        Currency = currency;
        StartDate = startDate;
        EndDate = endDate;
        InitialBalance = initialBalance;
        FinalBalance = finalBalance;
        Transactions = transactions;
    }

    private BankStatement(string accountHolder, string currency, DateTime startDate, DateTime endDate)
    {
        AccountHolder = accountHolder;
        Currency = currency;
        StartDate = startDate;
        EndDate = endDate;
        Transactions = [];
    }

    public static BankStatement Generate(BankAccount account, DateTime startDate, DateTime endDate)
    {
        BankStatement statement = new(account.AccountHolder, account.Currency, startDate, endDate)
        {
            // Obtém saldo antes do período
            InitialBalance = account.CalculateBalanceBefore(startDate),
            FinalBalance = account.CalculateBalanceBefore(startDate)
        };

        // Filtra e processa eventos de transação
        statement.ProcessTransactions(account.Events);

        return statement;
    }

    private void ProcessTransactions(IEnumerable<Event> events)
    {
        var filteredEvents = events
            .Where(e => e.TimeStamp >= StartDate && e.TimeStamp <= EndDate)
            .Where(e => e is MoneyDeposited || e is MoneyWithdrawn || e is MoneyTransferred)
            .OrderBy(e => e.TimeStamp);

        foreach (var e in filteredEvents)
        {
            TransactionType type;
            decimal amount;

            switch (e)
            {
                case MoneyDeposited d:
                    type = TransactionType.Deposit;
                    amount = d.Amount;
                    FinalBalance += d.Amount;
                    break;
                case MoneyWithdrawn w:
                    type = TransactionType.Withdrawal;
                    amount = w.Amount;
                    FinalBalance -= w.Amount;
                    break;
                case MoneyTransferred t:
                    type = TransactionType.Transfer;
                    amount = t.Amount;
                    FinalBalance -= t.Amount;
                    break;
                default:
                    continue;
            }

            Transactions.Add(BankTransaction.Generate(e.TimeStamp, type, amount, Currency, $"Bank statement of {StartDate} to {EndDate}"));
        }
    }

    public void PrintStatement()
    {
        Console.WriteLine($"EXTRATO BANCÁRIO\nTitular: {AccountHolder}");
        Console.WriteLine($"Período: {StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}");
        Console.WriteLine($"Saldo Inicial: {InitialBalance:C} {Currency}");
        Console.WriteLine("--------------------------------------------------");

        if (Transactions.Count != 0)
        {
            foreach (var transaction in Transactions)
            {
                Console.WriteLine(transaction);
            }
        }
        else
        {
            Console.WriteLine("Nenhuma transação registrada no período.");
        }

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Saldo Final: {FinalBalance:C} {Currency}");
    }
}

