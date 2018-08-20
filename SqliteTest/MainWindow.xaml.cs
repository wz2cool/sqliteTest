using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqliteTest.Models;
using SqliteTest.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqliteTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly string sqliteLiteFilepath = "d:\\test\\123.sqlite";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                InitData();
            });
        }

        private void InitData()
        {
            Console.WriteLine("InitData start");
            SQLiteConnection.CreateFile(sqliteLiteFilepath);
            IEnumerable<string> urls = GetUrls();
            foreach (string url in urls)
            {
                string json = GetJSON(url);
                var users = JsonConvert.DeserializeObject<List<User>>(json);
                Console.WriteLine(users.Count);
                SaveUsersToDB(users);
            }
            Console.WriteLine("InitData end");
        }

        private void SaveUsersToDB(IEnumerable<User> users)
        {
            string sql = "CREATE TABLE IF NOT EXISTS users (" +
                "id INTEGER PRIMARY KEY," +
                "avatar varchar(100)," +
                "county varchar(100)," +
                "email varchar(100)," +
                "title varchar(100)," +
                "firstName varchar(100)," +
                "lastName varchar(100)," +
                "street varchar(100)," +
                "zipCode varchar(100)," +
                "date varchar(100)," +
                "bs varchar(100)," +
                "catchPhrase varchar(100)," +
                "companyName varchar(100)," +
                "words varchar(100)," +
                "sentence varchar(100)" +
                ")";

            using (var conn = new SQLiteConnection("data source=" + sqliteLiteFilepath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                using (var trasaction = conn.BeginTransaction())
                {
                    foreach (var item in users)
                    {
                        cmd.CommandText = string.Format("INSERT INTO users VALUES ({0}, \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\", \"{14}\")",
                            item.Id,
                            item.Avatar,
                            item.County,
                            item.Email,
                            item.Title,
                            item.FirstName,
                            item.LastName,
                            item.Street,
                            item.ZipCode,
                            item.Date,
                            item.Bs,
                            item.CatchPhrase,
                            item.CompanyName,
                            item.Words,
                            item.Sentence);
                        cmd.ExecuteNonQuery();
                    }


                    trasaction.Commit();
                }

                conn.Close();
            }
        }

        private IEnumerable<string> GetUrls()
        {
            List<string> urls = new List<string>();
            for (int i = 1; i <= 49; i++)
            {
                string url = string.Format("https://raw.githubusercontent.com/wz2cool/fake-data/master/users/users_{0}.json", i);
                urls.Add(url);
            }
            return urls;
        }

        private string GetJSON(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //Console.WriteLine("{0} start", DateTime.Now.Millisecond);
            //List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            //using (var conn = new SQLiteConnection("data source=" + sqliteLiteFilepath))
            //{
            //    conn.Open();
            //    string sql = "select * from users where id > 10000 and firstName like 'H%' limit 100";
            //    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            //    {
            //        using (var rdr = cmd.ExecuteReader())
            //        {
            //            while (rdr.Read())
            //            {
            //                Dictionary<string, object> objDict = new Dictionary<string, object>();
            //                for (int i = 0; i < rdr.FieldCount; i++)
            //                {
            //                    objDict.Add(rdr.GetName(i), rdr.GetValue(i));
            //                }
            //                result.Add(objDict);
            //            }
            //        }
            //    }
            //}

            //string json = JsonConvert.SerializeObject(result, Formatting.None);
            //Console.WriteLine(json);
            //Console.WriteLine("{0} end", DateTime.Now.Millisecond);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var version = DbService.Instance.GetTableVersion("test");
            Console.WriteLine(string.Format("version: {0}", version));
        }

        private void InitDbService_Click(object sender, RoutedEventArgs e)
        {
            DbService.Instance.Init();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string json = "{'sqlExpression': 'SELECT * FROM users WHERE (id=? AND name=?)', params: [1, 'test']}";
            var value = JsonConvert.DeserializeObject<SqlTemplate>(json);
            DbService.Instance.GetSqliteTemplateWrapper(value);
        }
    }
}
