using Missions.Classes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Missions.Views
{
    public partial class EditProjectWin : Window
    {
        private int ProjectId { get; set; }

        public EditProjectWin(int projectId)
        {
            InitializeComponent();
            ProjectId = projectId;
        }

        public void LoadUsersAndSelectParticipants(List<int> selectedUserIds)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Загружаем всех пользователей и выделяем участников
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

                        // Если пользователь является участником проекта, выделяем его
                        if (selectedUserIds.Contains(user.IdUser))
                        {
                            UsersListBox.SelectedItems.Add(user);
                        }
                    }
                }
            }
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void deleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Хотите удалить данный проект?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteProjectFromDatabase(ProjectId);
                MessageBox.Show("Проект удален.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем доску после удаления проекта
                ((BoardWin)Owner).ReloadUserProjects();

                DialogResult = false;
                Close();
            }
        }


        private void DeleteProjectFromDatabase(int projectId)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    // Удаляем задачи проекта
                    string deleteTasksQuery = "DELETE FROM Tasks WHERE idProject = @idProject";
                    using (SQLiteCommand command = new SQLiteCommand(deleteTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    // Удаляем связи между задачами и пользователями
                    string deleteUserTaskQuery = "DELETE FROM User_Task WHERE idTask IN (SELECT idTask FROM Tasks WHERE idProject = @idProject)";
                    using (SQLiteCommand command = new SQLiteCommand(deleteUserTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    // Удаляем связи между проектом и пользователями
                    string deleteUserProjectQuery = "DELETE FROM User_Project WHERE idProject = @idProject";
                    using (SQLiteCommand command = new SQLiteCommand(deleteUserProjectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    // Удаляем проект
                    string deleteProjectQuery = "DELETE FROM Projects WHERE idProject = @idProject";
                    using (SQLiteCommand command = new SQLiteCommand(deleteProjectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
