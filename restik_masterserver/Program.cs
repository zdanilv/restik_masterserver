using SimpleHttp;

namespace restik_masterserver
{
    internal class Program
    {
        private static DBClient? DBClient { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Http Server 0.1");
            Console.WriteLine("Press ENTER to exit.");
            ConnectToSQLServer();
            Task task = Task.Factory.StartNew(() => Run());
            Console.WriteLine("Start server...");
            while (true)
            {
                Console.WriteLine("Query: ");
                string query = Console.ReadLine();
                if (query == "")
                {
                    break;
                }
                else DBClient.NonQuery(query);
            }
        }
        private static Task Run() // Запускаем сервер
        {
            try
            {
                Route.Add("/", (req, res, props) =>
                {
                    Console.WriteLine("{0}: OK - Incoming request - '/'.", DateTime.Now);
                    Console.WriteLine(req.ContentEncoding);
                    Console.WriteLine(req.HttpMethod);
                    Console.WriteLine(req.ProtocolVersion);
                    Console.WriteLine(req.Headers);
                    res.StatusCode = 200;
                    res.AsText("Welcome! Хай!");
                }, "POST");
                Route.Add("/sign/user={username};password={hash}", (req, res, props) =>
                {
                    if (props != null)
                    {
                        Console.WriteLine("{0}: OK - Incoming request - '/sign/'.", DateTime.Now);
                        Console.WriteLine(req.ContentEncoding);
                        Console.WriteLine(req.HttpMethod);
                        Console.WriteLine(req.ProtocolVersion);
                        Console.WriteLine(req.Headers);
                        res.StatusCode = 200;
                        res.AsBytes(req, DBClient.Login(props["username"], props["hash"]));
                    }
                }, "POST");
                HttpServer.ListenAsync(1337, CancellationToken.None, Route.OnHttpRequestAsync).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.CompletedTask;
            }
        }
        private static void ConnectToSQLServer() // Подключение к БД
        {
            try
            {
                DBClient = DBClient.GetInstance();
                DBClient.Connect("127.0.0.1", "49172");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}