# Event-Driven Bank Account Example

## Overview

This project demonstrates an event-driven domain model for a bank account system. It uses event sourcing to track changes in account states and allows for replaying past events to reconstruct the account's history.

## Features

- **Opening a bank account**: Create a new account with an initial deposit.
- **Bank deposit**: Add funds to an existing account.
- **Withdrawal**: Withdraw funds from an account.
- **Transfer between accounts**: Move funds between different accounts.
- **Closing the account**: Close an account under certain conditions.
- **Event replay**: Reconstruct account state from historical events.
- **Bank statement generation**: Create bank statements based on past transactions.

## Core Components

### `BankAccount`

The `BankAccount` class represents a bank account and maintains its state through event sourcing. Key methods include:

- `Open(accountHolder, initialDeposit, currency)`: Creates a new bank account.
- `Deposit(amount, description)`: Deposits money into the account.
- `Withdrawn(amount, description)`: Withdraws money from the account.
- `TransferTo(toAccountId, amount, description)`: Transfers money to another account.
- `Close(reason)`: Closes the account if conditions are met.
- `ReplayEvents(events)`: Reconstructs account state from past events.

### `BankStatement`

The `BankStatement` class generates account statements based on past transactions.

- Stores account details, transaction history, and balances.
- Filters and processes events to create a summary of account activity.

### `BankTransaction`

Represents a single transaction with:

- Timestamp
- Type (Deposit, Withdrawal, Transfer)
- Amount
- Currency
- Description

### Events

All account operations generate events that store immutable historical data:

- `AccountOpened`
- `MoneyDeposited`
- `MoneyWithdrawn`
- `MoneyTransferred`
- `AccountClosed`

Each event is timestamped and used to track account changes over time.

## Installation & Usage

1. Clone the repository:
   ```sh
   git clone https://github.com/guilherme-beltran/event-sourcing.git
   cd event-sourcing
   ```
2. Build the project:
   ```sh
   dotnet build
   ```
3. Run the application:
   ```sh
   dotnet run
   ```

## Running with Docker

To run the application inside a Docker container, follow these steps:

1. Build the Docker image:
   ```sh
   docker build -t event-sourcing .
   ```
2. Run the container:
   ```sh
   docker run --rm -p 8080:8080 event-sourcing
   ```
   
## License

This project is open-source and available under the MIT License.

