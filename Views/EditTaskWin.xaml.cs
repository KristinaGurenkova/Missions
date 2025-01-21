using Missions.Classes;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Missions.Views
{
    public partial class EditTaskWin : Window
    {
        private int ProjectId { get; set; }
        private int TaskId { get; set; }

        public EditTaskWin(int projectId, int taskId, string fullName)
        {
            InitializeComponent();
            ProjectId = projectId;
            TaskId = taskId;
            AssigneeComboBox.Text = fullName;
            LoadAssignees(projectId);
        }
        public void LoadAssignees(int projectId)
        {
            AssigneeComboBox.Items.Clear();
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT Users.* FROM Users 
                         INNER JOIN User_Project ON Users.idUser = User_Project.idUser 
                         WHERE User_Project.idProject = @projectId";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
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
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем окно и возвращаем DialogResult = true
            DialogResult = true;
            Close();
        }

        private void deleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Хотите удалить данную задачу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteTaskFromDatabase(TaskId);
                MessageBox.Show("Задача удалена.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем задачи на доске после удаления
                ((BoardWin)Owner).ReloadUserProjects();

                DialogResult = false;
                Close();
            }
        }

        private void DeleteTaskFromDatabase(int taskId)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    // Удаляем связь между задачей и пользователями
                    string deleteUserTaskQuery = "DELETE FROM User_Task WHERE idTask = @idTask";
                    using (SQLiteCommand command = new SQLiteCommand(deleteUserTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idTask", taskId);
                        command.ExecuteNonQuery();
                    }

                    // Удаляем задачу
                    string deleteTaskQuery = "DELETE FROM Tasks WHERE idTask = @idTask";
                    using (SQLiteCommand command = new SQLiteCommand(deleteTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idTask", taskId);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
