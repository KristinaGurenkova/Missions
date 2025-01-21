using Missions.Classes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Missions.Views
{
    public partial class CreateProjectWindow : Window
    {
        public string ProjectName => ProjectNameBox.Text;
        public string ProjectDescription => ProjectDescriptionBox.Text;
        public DateTime ProjectDeadline => DeadlinePicker.SelectedDate ?? DateTime.Now;
        public List<int> SelectedUsers => UsersListBox.SelectedItems.Cast<User>().Select(u => u.IdUser).ToList();

        private string CurrentUserLogin { get; }

        public CreateProjectWindow(string currentUserLogin)
        {
            InitializeComponent();
            LoadUsers();
            CurrentUserLogin = currentUserLogin;
        }

        private void LoadUsers()
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            IdUser = Convert.ToInt32(reader["idUser"]),
                            Login = reader["login"].ToString(),
                            Password = reader["password"].ToString(),
                            Surname = reader["surname"].ToString(),
                            Name = reader["name"].ToString(),
                            Middlename = reader["middlename"].ToString(),
                            Email = reader["email"].ToString(),
                            IdRole = Convert.ToInt32(reader["idRole"])
                        };
                        UsersListBox.Items.Add(user);
                    }
                }
            }
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ProjectDescription) || SelectedUsers.Count == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и выберите хотя бы одного пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
