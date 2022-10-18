using PaymentServices.Types;

namespace PaymentServices.Data;

public interface IAccountDataStore
{
    IEnumerable<Account> Accounts { get; set; }
    
    Account GetAccount(string accountNumber);

    void UpdateAccount(Account account);
}