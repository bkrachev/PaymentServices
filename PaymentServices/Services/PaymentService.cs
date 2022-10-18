using PaymentServices.Data;
using PaymentServices.Types;

namespace PaymentServices.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountDataStore accountDataStore;

    public PaymentService(IAccountDataStore accountDataStore)
    {
        this.accountDataStore = accountDataStore;
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        var account = accountDataStore.GetAccount(request.DebtorAccountNumber);
        var result = new MakePaymentResult();

        PaymentScheme paymentScheme = request.PaymentScheme;
        string paymentKey = Enum.GetName<PaymentScheme>(paymentScheme);
        AllowedPaymentSchemes allowedPaymentScheme = Enum.Parse<AllowedPaymentSchemes>(paymentKey);
        
        if (account == null || 
            !account.AllowedPaymentScheme.Equals(allowedPaymentScheme) ||
            (request.PaymentScheme == PaymentScheme.FasterPayments && account.Balance < request.Amount) ||
            (request.PaymentScheme == PaymentScheme.Chaps && account.Status != AccountStatus.Live))
        {
            result.Success = false;
        }

        if (result.Success)
        {
            account.Balance -= request.Amount;
            accountDataStore.UpdateAccount(account);
        }

        return result;
    }
}