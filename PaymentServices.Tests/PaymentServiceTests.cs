using PaymentServices.Data;
using PaymentServices.Services;
using PaymentServices.Types;

namespace PaymentServices.Tests;

public class Tests
{
    private const string AccountNumberOne = "11";
    private const string AccountNumberTwo = "22";
    private const string AccountNumberThree = "33";
    
    private const decimal Balance = 50m;
    
    private const AllowedPaymentSchemes Bacs = AllowedPaymentSchemes.Bacs;
    private const AllowedPaymentSchemes Chaps = AllowedPaymentSchemes.Chaps;
    private const AllowedPaymentSchemes Faster = AllowedPaymentSchemes.FasterPayments;

    private const PaymentScheme BacsScheme = PaymentScheme.Bacs;
    private const PaymentScheme ChapsScheme = PaymentScheme.Chaps;
    private const PaymentScheme FasterScheme = PaymentScheme.FasterPayments;

    private const AccountStatus LiveStatus = AccountStatus.Live;
    private const AccountStatus DisabledStatus = AccountStatus.Disabled;
    private const AccountStatus InboundStatus = AccountStatus.InboundPaymentsOnly;

    private Account accountOne;
    private Account accountTwo;
    private Account accountThree;
    
    private ICollection<Account> accounts;
    private IAccountDataStore accountDataStore;
    private IPaymentService service;

    [SetUp]
    public void Setup()
    {
        accountOne = new Account()
        {
            AccountNumber = AccountNumberOne,
            Balance = Balance,
            Status = LiveStatus,
            AllowedPaymentScheme = Chaps
        };

        accountTwo = new Account()
        {
            AccountNumber = AccountNumberTwo,
            Balance = Balance,
            Status = LiveStatus,
            AllowedPaymentScheme = Bacs
        };

        accountThree = new Account()
        {
            AccountNumber = AccountNumberThree,
            Balance = Balance,
            Status = InboundStatus,
            AllowedPaymentScheme = Faster
        };

        accounts = new List<Account>();
        accounts.Add(accountOne);
        accounts.Add(accountTwo);
        accounts.Add(accountThree);
    }

    [Test]
    public void AccountNullPaymentTest()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Backup");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);
        var paymentRequest = new MakePaymentRequest { DebtorAccountNumber = "1234" };
        var paymentResult = service.MakePayment(paymentRequest);
        Assert.IsFalse(paymentResult.Success);
    }

    [Test]
    public void PaymentFailsWhenSchemesNotMatch()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var paymentRequest = new MakePaymentRequest()
        {
            Amount = 10,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberTwo,
            PaymentDate = DateTime.Now,
            PaymentScheme = FasterScheme
        };

        var result = service.MakePayment(paymentRequest);

        Assert.IsFalse(result.Success);
    }

    [Test]
    public void PaymentFailsWhenBalanceLow()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var paymentRequest = new MakePaymentRequest()
        {
            Amount = 100,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberTwo,
            PaymentDate = DateTime.Now,
            PaymentScheme = FasterScheme
        };

        var result = service.MakePayment(paymentRequest);

        Assert.IsFalse(result.Success);
    }

    [Test]
    public void PaymentFailsWhenAccountStatusNotLive()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var paymentRequest = new MakePaymentRequest()
        {
            Amount = 10,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberTwo,
            PaymentDate = DateTime.Now,
            PaymentScheme = ChapsScheme
        };

        var result = service.MakePayment(paymentRequest);

        Assert.IsFalse(result.Success);
    }

    [Test]
    public void PaymentSuccessfulBacs()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var request = new MakePaymentRequest()
        {
            Amount = 10,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberTwo,
            PaymentDate = DateTime.Now,
            PaymentScheme = BacsScheme
        };

        var result = service.MakePayment(request);

        Assert.IsTrue(result.Success);
    }
    
    [Test]
    public void PaymentSuccessfulChaps()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var request = new MakePaymentRequest()
        {
            Amount = 10,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberOne,
            PaymentDate = DateTime.Now,
            PaymentScheme = ChapsScheme
        };

        var result = service.MakePayment(request);

        Assert.IsTrue(result.Success);
    }
    
    [Test]
    public void PaymentSuccessfulFast()
    {
        accountDataStore = new DataStoreFactory().GetAccountDataStore("Normal");
        accountDataStore.Accounts = accounts;
        service = new PaymentService(accountDataStore);

        var request = new MakePaymentRequest()
        {
            Amount = 10,
            CreditorAccountNumber = AccountNumberOne,
            DebtorAccountNumber = AccountNumberThree,
            PaymentDate = DateTime.Now,
            PaymentScheme = FasterScheme
        };

        var result = service.MakePayment(request);

        Assert.IsTrue(result.Success);
    }
}