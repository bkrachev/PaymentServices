using PaymentServices.Types;

namespace PaymentServices.Data;

public class AccountDataStore : IAccountDataStore
{
    public IEnumerable<Account> Accounts { get; set; }

    public Account GetAccount(string accountNumber)
    {
        return Accounts.FirstOrDefault(x => x.AccountNumber == accountNumber);
    }

    public void UpdateAccount(Account account)
    {
    }
}