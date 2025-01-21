using Missions.Classes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Missions.Views
{
    /// <summary>
    /// Логика взаимодействия для RestoreWin.xaml
    /// </summary>
    public partial class RestoreWin : Window
    {
        public RestoreWin()
        {
            InitializeComponent();
        }

        private void replacePassword_Click(object sender, RoutedEventArgs e)
        {
            if (mailBox.Text != "" && passBox.Password != "")
            {

                string connectionString = "Data Source=Missions.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET password = @password WHERE email = @email";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@password", passBox.Password);
                        command.Parameters.AddWithValue("@email", mailBox.Text);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Пароль изменен!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите необходимые данные!");
            }
        }
    }
}
