// Annette Hawks
// Lab 11: ATM Machine
//Due 7/30/2027


class Program
{
    static void Main(string[] args)
    {
        // STEP 1: Get the list of customers from bank.txt
        // We need usernames, pins, and balances to make the ATM work
        List<Customer> customers = LoadBankCustomers("bank.txt");

        // STEP 2: Make sure the person logs in before doing anything
        // They only get 3 tries or they get locked out
        Customer currentUser = ValidateUser(customers);
        if (currentUser == null)
        {
            Console.WriteLine("Too many failed attempts. Try again later.");
            return;
        }

        // STEP 3: Main ATM menu – loop until they choose to end session
        // This is where they can make their selections after loggin in 
        List<string> transactionHistory = new List<string>();
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nATM Menu:");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Display Last 5 Transactions");
            Console.WriteLine("5. Quick Withdraw $20");
            Console.WriteLine("6. Quick Withdraw $40");
            Console.WriteLine("7. Quick Withdraw $100");
            Console.WriteLine("8. End Session");
            Console.Write("Menu Selction: 1-8 ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CheckBalance(currentUser); // just shows the balance
                    break;
                case "2":
                    WithdrawMoney(currentUser, transactionHistory); // removes money (if they have it)
                    break;
                case "3":
                    DepositMoney(currentUser, transactionHistory); // adds money (if valid)
                    break;
                case "4":
                    DisplayTransactions(transactionHistory); // shows last 5 things they did
                    break;
                case "5":
                    QuickWithdraw(currentUser, 20, transactionHistory); // fast cash $20
                    break;
                case "6":
                    QuickWithdraw(currentUser, 40, transactionHistory); // fast cash $40
                    break;
                case "7":
                    QuickWithdraw(currentUser, 100, transactionHistory); // fast cash $100
                    break;
                case "8":
                    // End session – save their balance back to the file
                    SaveBankCustomers("bank.txt", customers);
                    running = false;
                    Console.WriteLine("Thank you, Come again.");
                    break;
                default:
                    Console.WriteLine("He chose...poorly. Goodbye.");
                    break;
            }
        }
    }

    // Read all customers from the text file and build Customer objects
    static List<Customer> LoadBankCustomers(string filename)
    {
        List<Customer> customers = new List<Customer>();
        if (File.Exists(filename))
        {
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    string username = parts[0].Trim();
                    string pin = parts[1].Trim();
                    string balanceString = parts[2].Trim().Replace("$", "");
                    if (decimal.TryParse(balanceString, out decimal balance))
                    {
                        customers.Add(new Customer(username, pin, balance));
                    }
                }
            }
        }
        return customers;
    }

    // Save updated balances back to the file so they can be seen alter
    static void SaveBankCustomers(string filename, List<Customer> customers)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (Customer c in customers)
            {
                writer.WriteLine($"{c.Username},{c.Pin},${c.Balance:F2}");
            }
        }
    }

    // Let the user log in – return their bank information
    static Customer ValidateUser(List<Customer> customers)
    {
        int attempts = 0;
        while (attempts < 3)
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter PIN: ");
            string pin = Console.ReadLine();

            foreach (Customer customer in customers)
            {
                if (customer.Username == username && customer.Pin == pin)
                {
                    Console.WriteLine($"You have chosen...wisely, {username}!");
                    return customer;
                }
            }
            Console.WriteLine("Wrong login. Try again.");
            attempts++;
        }
        return null;
    }

    // Just shows the user’s current balance
    static void CheckBalance(Customer user)
    {
        Console.WriteLine($"Your current balance is: ${user.Balance:F2}");
    }

    // Subtracts money from balance (if valid), and logs the transaction
    static void WithdrawMoney(Customer user, List<string> history)
    {
        Console.Write("Enter amount to withdraw: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            if (amount <= 0)
            {
                Console.WriteLine("Nope, nice try, but you can’t withdraw a negative amount.");
            }
            else if (amount > user.Balance)
            {
                Console.WriteLine("You don’t have enough money for that.");
            }
            else
            {
                user.Balance -= amount;
                history.Add($"Withdrew ${amount:F2}");
                Console.WriteLine($"New balance: ${user.Balance:F2}");
            }
        }
        else
        {
            Console.WriteLine("That wasn’t a valid number.");
        }
    }

    // Adds money to balance (if valid), and logs the transaction
    static void DepositMoney(Customer user, List<string> history)
    {
        Console.Write("Enter amount to deposit: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            if (amount <= 0)
            {
                Console.WriteLine("You can’t deposit a negative amount.");
            }
            else
            {
                user.Balance += amount;
                history.Add($"Deposited ${amount:F2}");
                Console.WriteLine($"New balance: ${user.Balance:F2}");
            }
        }
        else
        {
            Console.WriteLine("That wasn’t a valid number.");
        }
    }

    // Fast cash option for set amounts – only works if you have enough
    static void QuickWithdraw(Customer user, int amount, List<string> history)
    {
        if (user.Balance >= amount)
        {
            user.Balance -= amount;
            history.Add($"Quick withdrew ${amount}");
            Console.WriteLine($"Quick withdraw successful. New balance: ${user.Balance:F2}");
        }
        else
        {
            Console.WriteLine("You're too poor for that quick withdraw.");
        }
    }

    // Show the most recent 5 things they did (or less if they didn’t do that many)
    static void DisplayTransactions(List<string> history)
    {
        Console.WriteLine("Last 5 Transactions:");
        int count = Math.Min(history.Count, 5);
        for (int i = history.Count - count; i < history.Count; i++)
        {
            Console.WriteLine(history[i]);
        }
    }
}

// Just a simple object to hold username, pin, and balance
class Customer
{
    public string Username { get; set; }
    public string Pin { get; set; }
    public decimal Balance { get; set; }

    public Customer(string username, string pin, decimal balance)
    {
        Username = username;
        Pin = pin;
        Balance = balance;
    }
}
