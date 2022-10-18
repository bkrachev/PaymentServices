using System.Configuration;

namespace PaymentServices.Data;

public class DataStoreFactory
{
    public IAccountDataStore GetAccountDataStore(string dataStoreType = null)
    {
        if (string.IsNullOrWhiteSpace(dataStoreType))
        {
            dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
        }

        if (dataStoreType == "Backup")
        {
            return new BackupAccountDataStore();
        }

        return new AccountDataStore();
    }
}