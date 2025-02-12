using EventSourcing.Events;
using EventSourcing.Results;

namespace EventSourcing.BankAccounts;

public class BankAccount : Entity
{
    public string AccountHolder { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }
    public bool IsActive { get; private set; }

    private BankAccount() { }

    public static BankAccount Open(
        string accountHolder,
        decimal initialDeposit,
        string currency = "BRL")
    {
        if (string.IsNullOrWhiteSpace(accountHolder))
        {
            throw new ArgumentException("Account holder name is required");
        }

        if (initialDeposit < 0)
        {
            throw new ArgumentException("The initial deposit can't be negative");
        }

        BankAccount bankAccount = new();

        var @event = new AccountOpened(
            AccountId: bankAccount.Id,
            AccountHolder: accountHolder,
            InitialDeposit: initialDeposit,
            Currency: currency);

        bankAccount.Apply(@event);

        return bankAccount;
    }

    public Result Deposit(decimal amount, string description)
    {
        var ensure = EnsureAccountIsActive();
        if (ensure.IsFailure)
        {
            return ensure.Error;
        }

        if (amount <= 0)
        {
            return new Error("BankAccount.Deposit.Amount", "Deposit amount must be positive");
        }

        var apply = Apply(new MoneyDeposited(Id, amount, description));
        return apply;
    }

    public Result Withdrawn(decimal amount, string description)
    {
        var ensure = EnsureAccountIsActive();
        if (ensure.IsFailure)
        {
            return ensure.Error;
        }

        if (amount <= 0)
        {
            return new Error("BankAccount.Withdrawn.Amount", "Withdrawn amount must be positive");
        }

        if (Balance - amount < 0)
        {
            return new Error("BankAccount.Withdrawn.Balance", "Insufficient funds");
        }

        var apply = Apply(new MoneyWithdrawn(Id, amount, description));
        return apply;
    }

    public Result TransferTo(Guid toAccountId, decimal amount, string description)
    {
        var ensure = EnsureAccountIsActive();
        if (ensure.IsFailure)
        {
            return ensure.Error;
        }

        if (amount <= 0)
        {
            return new Error("BankAccount.TransferTo.Amount", "Withdrawn amount must be positive");
        }

        if (Balance - amount <= 0)
        {
            return new Error("BankAccount.TransferTo.Balance", "Insufficient funds");
        }

        var apply = Apply(new MoneyTransferred(Id, amount, toAccountId, description));
        return apply;
    }

    public Result Close(string reason)
    {
        var ensure = EnsureAccountIsActive();
        if (ensure.IsFailure)
        {
            return new Error("BankAccount.Close.IsActive", "Account already closed");
        }

        if (Balance > 0)
        {
            return new Error("BankAccount.Close.Balance", "Can't close an account with a balance");
        }
        
        if (Balance < 0)
        {
            return new Error("BankAccount.Close.Balance", "Can't close an account with a negative balance");
        }

        var apply = Apply(new AccountClosed(Id, reason));
        return apply;

    }

    public decimal CalculateBalanceBefore(DateTime date)
    {
        var balance = Events
            .Where(e => e.TimeStamp < date)
            .Sum(e => e switch
            {
                MoneyDeposited d => d.Amount,
                MoneyWithdrawn w => -w.Amount,
                MoneyTransferred t => -t.Amount,
                _ => 0
            });
        return balance;
    }

    private Result Apply(Event @event)
    {
        Result result = @event switch
        {
            AccountOpened e => OpenAccount(e),
            MoneyDeposited e => DepositMoney(e),
            MoneyWithdrawn e => WithdrawnMoney(e),
            MoneyTransferred e => TransferMoney(e),
            AccountClosed => CloseAccount(),
            _ => new Error("Apply.@event", "Unmapped event to be applied")
        };

        if (result.IsFailure)
        {
            return result.Error;
        }

        EventGenerated(@event);

        return result;
    }

    public static BankAccount ReplayEvents(IEnumerable<Event> events)
    {
        var bankAccount = new BankAccount();
        foreach (var @event in events)
        {
            bankAccount.Apply(@event);
        }

        return bankAccount;
    }

    public Result EnsureAccountIsActive()
    {
        if (!IsActive)
        {
            return new Error("BankAccount.EnsureAccountIsActive.IsActive", "Account is closed");
        }

        return Result.Success();
    }


    #region Behaviors events

    private Result CloseAccount()
    {
        IsActive = false;
        return Result.Success();
    }

    private Result TransferMoney(MoneyTransferred e)
    {
        if (e is null)
        {
            return new Error("BankAccount.TransferMoney", "Invalid event");
        }

        Balance -= e.Amount;
        return Result.Success();
    }

    private Result WithdrawnMoney(MoneyWithdrawn e)
    {
        if (e is null)
        {
            return new Error("BankAccount.WithdrawnMoney", "Invalid event");
        }

        Balance -= e.Amount;
        return Result.Success();
    }

    private Result DepositMoney(MoneyDeposited e)
    {
        if (e is null)
        {
            return new Error("BankAccount.DepositMoney", "Invalid event");
        }

        Balance += e.Amount;
        return Result.Success();
    }

    private Result OpenAccount(AccountOpened e)
    {
        if (e is null)
        {
            return new Error("BankAccount.OpenAccount", "Invalid event");
        }

        AccountHolder = e.AccountHolder;
        Balance = e.InitialDeposit;
        Currency = e.Currency;
        IsActive = true;
        return Result.Success();
    }

    #endregion
}
