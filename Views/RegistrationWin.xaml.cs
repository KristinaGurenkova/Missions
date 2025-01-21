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

namespace Missions.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWin.xaml
    /// </summary>
    public partial class RegistrationWin : Window
    {
        MainWindow mainWindow = new MainWindow();
        public RegistrationWin()
        {
            InitializeComponent();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        private void signIn_Click(object sender, RoutedEventArgs e)
        {
            if (loginBox.Text != ""
                && passBox.Password != ""
                && surnameBox.Text != ""
                && nameBox.Text != ""
                && patronymicBox.Text != ""
                && mailBox.Text != "")
            {
                using (var connection = new SQLiteConnection("Data Source=Missions.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"INSERT INTO Users (login, password,surname,name,middlename,email,idRole) 
                    VALUES (@login, @password, @surname, @name, @middlename, @email, @idRole);";

                    command.Parameters.AddWithValue("@login", loginBox.Text);
                    command.Parameters.AddWithValue("@password", passBox.Password);
                    command.Parameters.AddWithValue("@surname", surnameBox.Text);
                    command.Parameters.AddWithValue("@name", nameBox.Text);
                    command.Parameters.AddWithValue("@middlename", patronymicBox.Text);
                    command.Parameters.AddWithValue("@email", mailBox.Text);
                    command.Parameters.AddWithValue("@idRole", 2);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Пользователь зарегистрирован!");
                    this.Close();
                    mainWindow.Show();
                }
            }
            else
            {
                MessageBox.Show("Введите все данные!");
            }
        }
    }
}
