using System;
using System.Data.SQLite;
using System.Windows;
using Missions.Views;

namespace Missions
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void signUp_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWin registrationWin = new RegistrationWin();
            registrationWin.Show();
            this.Close();
        }

        private void forget_Click(object sender, RoutedEventArgs e)
        {
            RestoreWin restoreWin = new RestoreWin();
            restoreWin.Show();
            this.Close();
        }

        private void logIn_Click(object sender, RoutedEventArgs e)
        {
            // Создание и открытие соединения с базой данных
            using (var connection = new SQLiteConnection("Data Source=Missions.db"))
            {
                connection.Open();

                // Создание команды SQL для получения данных пользователя
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Users.*, Roles.nameRole 
                FROM Users 
                INNER JOIN Roles ON Users.idRole = Roles.idRole
                WHERE login = @login AND password = @password";

                // Добавление параметров в команду SQL
                command.Parameters.AddWithValue("@login", loginBox.Text);
                command.Parameters.AddWithValue("@password", passBox.Password);

                // Выполнение команды и обработка результатов
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Если пользователь найден
                    {
                        // Получение роли пользователя
                        var role = reader["nameRole"].ToString();

                        // Открытие соответствующего окна в зависимости от роли пользователя
                        switch (role)
                        {
                            case "Менеджер":
                                // Открываем окно для менеджера
                                ManagerWin managerWin = new ManagerWin(loginBox.Text);
                                managerWin.Show();
                                Close();
                                break;
                            case "Пользователь":
                                // Открываем окно для пользователя, передавая логин
                                BoardWin boardWin = new BoardWin(loginBox.Text);
                                boardWin.Show();
                                Close();
                                break;
                            default:
                                MessageBox.Show("Неизвестная роль пользователя");
                                break;
                        }
                    }
                    else // Если пользователь не найден
                    {
                        // Вывод сообщения об ошибке
                        MessageBox.Show("Неверный логин или пароль");
                    }
                }
            }
        }
    }
}
