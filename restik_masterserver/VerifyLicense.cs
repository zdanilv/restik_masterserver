using System.Data.SqlClient;

namespace restik_masterserver
{
    internal class VerifyLicense
    {
        private static Timer _timer;
        private static License? License { get; set; }
        private static volatile VerifyLicense? _instance;
        private static readonly object _lock = new object();
        public static VerifyLicense GetInstance()
        {
            if (_instance != null)
                return _instance;
            lock (_lock)
                if (_instance == null)
                    _instance = new VerifyLicense();
            return _instance;
        }
        public VerifyLicense()
        {
            Thread.Sleep(5000);
            TimerCallback timerCallback = new TimerCallback(Verify);
            _timer = new Timer(timerCallback, 0, 0, 1000*60*60*24); // Каждые 24 часа проверка лицензии
        }
        private static void Verify(object obj) // Проверка лицензии и изменение активности лицензии, если период закончился
        {
            try
            {
                DBClient dbClient = DBClient.GetInstance();
                using (SqlCommand sqlCommand = new SqlCommand("SELECT ID, ExpirationLicense, LicenseActive FROM Users", dbClient.SqlConnection))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            License = new License
                            {
                                ID = (int)reader["ID"],
                                ExpirationLicense = (DateTime)reader["ExpirationLicense"],
                                LicenseActive = (int)reader["LicenseActive"] == 1 ? true : false
                            };
                            if (License.LicenseActive == true && License.ExpirationLicense < DateTime.Today)
                            {
                                dbClient.NonQuery($"UPDATE Users SET LicenseActive=0 WHERE ID={License.ID}");
                                Console.WriteLine($"License expired ID - {License.ID}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: ERROR - Error during verification.", DateTime.Now);
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("{0}: OK - Verify was successful.", DateTime.Now);
        }
    }
}
