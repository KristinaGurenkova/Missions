using Missions.Classes;
using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Missions.Views
{
    public partial class ManagerWin : Window
    {
        static public string CurrentUserLogin { get; set; }

        public ManagerWin(string userLogin)
        {
            InitializeComponent();
            logBox.Text = userLogin;
            CurrentUserLogin = userLogin;
            LoadUsers();
        }

        private void LoadUsers()
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT Users.idUser, Users.login, Users.surname, Users.name, Users.middlename, Roles.nameRole 
                         FROM Users 
                         INNER JOIN Roles ON Users.idRole = Roles.idRole";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32(reader.GetOrdinal("idUser"));
                        string userLogin = reader.GetString(reader.GetOrdinal("login"));
                        string surname = reader.IsDBNull(reader.GetOrdinal("surname")) ? "NULL" : reader.GetString(reader.GetOrdinal("surname"));
                        string name = reader.IsDBNull(reader.GetOrdinal("name")) ? "NULL" : reader.GetString(reader.GetOrdinal("name"));
                        string middlename = reader.IsDBNull(reader.GetOrdinal("middlename")) ? "NULL" : reader.GetString(reader.GetOrdinal("middlename"));
                        string userRole = reader.GetString(reader.GetOrdinal("nameRole"));

                        string userName = $"{surname} {name} {middlename}";

                        StackPanel userPanel = CreateUserPanel(userId, userLogin, userName, userRole);
                        UsersPanel.Children.Add(userPanel);
                    }
                }

                query = @"
                SELECT email 
                FROM Users
                WHERE login = @login";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", CurrentUserLogin);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emailBox.Text = reader["email"].ToString();
                        }
                    }

                }
            }
        }
        private StackPanel CreateUserPanel(int userId, string userLogin, string userName, string userRole)
        {
            StackPanel userPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock loginText = new TextBlock
            {
                Text = userLogin,
                Width = 160,
                Margin = new Thickness(5),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock nameText = new TextBlock
            {
                Text = userName,
                Width = 350,
                Margin = new Thickness(5),
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };

            ComboBox roleComboBox = new ComboBox
            {
                Width = 210,
                Margin = new Thickness(5),
                ItemsSource = new List<string> { "Пользователь", "Менеджер" },
                FontSize = 20,
                SelectedItem = userRole
            };

            Button saveButton = new Button
            {
                Content = "Сохранить",
                Width = 150,
                Margin = new Thickness(5),
                FontSize = 20,
                Tag = userId
            };

            saveButton.Click += (s, e) => SaveUserRole(userId, roleComboBox.SelectedItem.ToString());

            userPanel.Children.Add(loginText);
            userPanel.Children.Add(nameText);
            userPanel.Children.Add(roleComboBox);
            userPanel.Children.Add(saveButton);

            return userPanel;
        }
        private void SaveUserRole(int userId, string newRole)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Получаем idRole для новой роли
                int newRoleId;
                string getRoleIdQuery = "SELECT idRole FROM Roles WHERE nameRole = @nameRole";
                using (SQLiteCommand command = new SQLiteCommand(getRoleIdQuery, connection))
                {
                    command.Parameters.AddWithValue("@nameRole", newRole);
                    newRoleId = Convert.ToInt32(command.ExecuteScalar());
                }

                // Обновляем только роль пользователя
                string updateUserRoleQuery = "UPDATE Users SET idRole = @idRole WHERE idUser = @idUser";
                using (SQLiteCommand command = new SQLiteCommand(updateUserRoleQuery, connection))
                {
                    command.Parameters.AddWithValue("@idRole", newRoleId);
                    command.Parameters.AddWithValue("@idUser", userId);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Роль пользователя обновлена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
