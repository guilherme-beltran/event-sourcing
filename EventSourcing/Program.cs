using EventSourcing.BankAccounts;

BankAccount bankAccount = OpenAndOperateAccount();

//LogEventsAccount(bankAccount);

GetBankStatement(bankAccount);

//ReplayEvents(bankAccount);

static BankAccount OpenAndOperateAccount()
{
    Console.WriteLine("Opening account...");
    var bankAccount = BankAccount.Open("G B", 0);
    Console.WriteLine($"Account opened. Initial balance: {bankAccount.Balance}");

    var anotherAccount = Guid.NewGuid();

    Console.WriteLine("Depositing 500...");
    bankAccount.Deposit(500, "Salary deposit");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine("Withdrawal 200...");
    bankAccount.Withdrawn(200, "ATM withdrawal");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine("transferring 100...");
    bankAccount.TransferTo(anotherAccount, 100, "Transfer to savings");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine($"Withdrawal 50...");
    bankAccount.Withdrawn(50, "Withdrawing before closing account");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine("Depositing 500...");
    bankAccount.Deposit(500, "Salary deposit");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine("Depositing 500...");
    bankAccount.Deposit(500, "Salary deposit");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    Console.WriteLine("transferring 400...");
    bankAccount.TransferTo(anotherAccount, 400, "Transfer to savings");
    Console.WriteLine($"Balance: {bankAccount.Balance}");

    bankAccount.Close("Completing demo");
    Console.WriteLine("Closing..");
    Console.WriteLine($"Final balance: {bankAccount.Balance}");
    return bankAccount;
}

static void LogEventsAccount(BankAccount bankAccount)
{
    foreach (var @event in bankAccount.Events)
    {
        Console.WriteLine($"Event: {@event.GetType().Name} at {@event.TimeStamp}");
    }
}

static void GetBankStatement(BankAccount bankAccount)
{
    var startDate = DateTime.UtcNow.AddHours(-1);
    var endDate = DateTime.UtcNow;
    var statement = BankStatement.Generate(bankAccount, startDate, endDate);
    statement.PrintStatement();
}

static void ReplayEvents(BankAccount bankAccount)
{
    var events = bankAccount.Events;

    var sameAccount = BankAccount.ReplayEvents(events);

    try
    {
        var result = sameAccount.Deposit(100, "Replaying deposit");
        if (result.IsFailure)
        {
            Console.WriteLine(result.Error.Message);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}