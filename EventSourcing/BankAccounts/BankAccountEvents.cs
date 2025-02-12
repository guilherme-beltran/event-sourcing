using EventSourcing.Events;

namespace EventSourcing.BankAccounts;

public record AccountOpened(
    Guid AccountId,
    string AccountHolder,
    decimal InitialDeposit,
    string Currency = "BRL") : Event(AccountId);

public record MoneyDeposited(
    Guid AccountId,
    decimal Amount,
    string Description) : Event(AccountId);

public record MoneyWithdrawn(
    Guid AccountId,
    decimal Amount,
    string Description) : Event(AccountId);

public record MoneyTransferred(
    Guid AccountId,
    decimal Amount,
    Guid ToAccountId,
    string Description) : Event(AccountId);

public record AccountClosed(
    Guid AccountId,
    string Reason) : Event(AccountId);
