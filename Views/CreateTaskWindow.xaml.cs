using Missions.Classes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Missions.Views
{
    public partial class CreateTaskWindow : Window
    {
        private int ProjectId { get; set; }

        public string TaskName => TaskNameBox.Text;
        public string TaskDescription => TaskDescriptionBox.Text;
        public DateTime TaskDeadline => TaskDeadlinePicker.SelectedDate ?? DateTime.Now;
        public int TaskAssigneeId => (AssigneeComboBox.SelectedItem as User)?.IdUser ?? -1;

        public CreateTaskWindow(int projectId)
        {
            InitializeComponent();
            ProjectId = projectId;
            LoadAssignees();
        }

        public void LoadAssignees()
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT Users.* FROM Users 
                                 INNER JOIN User_Project ON Users.idUser = User_Project.idUser 
                                 WHERE User_Project.idProject = @projectId";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", ProjectId);
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
                            AssigneeComboBox.Items.Add(user);
                        }
                    }
                }
            }

            //AssigneeComboBox.DisplayMemberPath = "Surname"; // Устанавливаем отображение логинов пользователей
        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskName) || string.IsNullOrWhiteSpace(TaskDescription) || TaskAssigneeId == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и выберите исполнителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
