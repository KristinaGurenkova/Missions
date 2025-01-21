using Missions.Classes;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Missions.Views
{
    public partial class BoardWin : Window
    {
        private string CurrentUserLogin { get; }

        public BoardWin(string userLogin)
        {
            InitializeComponent();
            CurrentUserLogin = userLogin;
            LoadUserProjects();
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow createProjectWindow = new(CurrentUserLogin);
            if (createProjectWindow.ShowDialog() == true)
            {
                string projectName = createProjectWindow.ProjectName;
                string projectDescription = createProjectWindow.ProjectDescription;
                DateTime projectDeadline = createProjectWindow.ProjectDeadline;
                var selectedUsers = createProjectWindow.SelectedUsers;

                if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(projectDescription) || selectedUsers.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля и выберите хотя бы одного пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int projectId = (int)SaveProjectToDatabase(projectName, projectDescription, projectDeadline, selectedUsers);

                ReloadUserProjects();
            }
        }


        private void LoadProject(string projectName, string projectDescription, DateTime projectDeadline, object selectedUsers, int projectId)
        {
            Border projectBorder = new Border
            {
                Style = (Style)Application.Current.Resources["RoundedBorderStyle"],
                Margin = new Thickness(10, 10, 10, 10)
            };

            StackPanel projectPanel = new StackPanel();
            projectPanel.Margin = new Thickness(10, 10, 10, 10);
            projectBorder.Child = projectPanel;

            TextBlock name = new TextBlock()
            {
                Text = projectName,
                FontSize = 40,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand,
            };

            TextBlock description = new TextBlock()
            {
                Text = projectDescription,
                FontSize = 27,
            };

            TextBlock date = new TextBlock()
            {
                Text = projectDeadline.ToString(),
                FontSize = 20,
                Padding = new Thickness(0, 10, 0, 0),
            };

            projectPanel.Children.Add(name);
            projectPanel.Children.Add(description);
            projectPanel.Children.Add(date);
            name.MouseLeftButtonDown += (s, e) => OpenEditProjectWindow(projectId, name, description, date);

            Button createTaskButton = new Button
            {
                Content = " Создать задачу ",
                Height = 33,
                FontSize = 20,
                Background = (Brush)new BrushConverter().ConvertFrom("White"),
                HorizontalAlignment = HorizontalAlignment.Left,
                BorderThickness = new Thickness(0, 0, 0, 0)
            };

            createTaskButton.Margin = new Thickness(0, 10, 0, 0);
            createTaskButton.Click += (s, args) => CreateTaskWindowHandler(projectPanel, projectId);
            projectPanel.Children.Add(createTaskButton);

            ProjectsPanel.Children.Add(projectBorder);

            LoadProjectTasks(projectPanel, projectId);
        }

        private StackPanel CreateTaskPanel(string taskName, string taskDescription, DateTime taskDeadline, string taskAssigneeLogin, bool isChecked, int taskId, int projectId, string fullName)
        {
            StackPanel taskPanel = new StackPanel { Orientation = Orientation.Horizontal };

            TextBlock task = new TextBlock()
            {
                Text = taskName + " ",
                FontSize = 20,
                Cursor = Cursors.Hand
            };

            TextBlock descriptionTask = new TextBlock()
            {
                Text = taskDescription + " ",
                FontSize = 20, 
            };

            TextBlock assignee = new TextBlock()
            {
                Text = $"Исполнитель: {fullName} ",
                FontSize = 20,
            };

            TextBlock dateTask = new TextBlock()
            {
                Text = taskDeadline.ToString(),
                FontSize = 20,
            };

            CheckBox taskStatus = new CheckBox();
            taskStatus.IsEnabled = taskAssigneeLogin == CurrentUserLogin;
            taskStatus.IsChecked = isChecked;
            taskStatus.Checked += (s, e) => UpdateTaskStatus(taskName, true);
            taskStatus.Unchecked += (s, e) => UpdateTaskStatus(taskName, false);
            taskStatus.Margin = new Thickness(0, 6, 6, 0);
            taskStatus.HorizontalContentAlignment = HorizontalAlignment.Left;

            task.MouseLeftButtonDown += (s, e) => OpenEditTaskWindow(taskId, task, descriptionTask, dateTask, assignee, taskStatus, projectId, fullName);

            taskPanel.Children.Add(taskStatus);
            taskPanel.Children.Add(task);
            taskPanel.Children.Add(assignee);
            taskPanel.Children.Add(dateTask);
            taskPanel.Margin = new Thickness(0, 10, 0, 0);

            return taskPanel;
        }

        private void LoadProjectTasks(StackPanel projectPanel, int projectId)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                SELECT Tasks.*, Users.login, Users.surname, Users.name, Users.middlename 
                FROM Tasks 
                INNER JOIN User_Task ON Tasks.idTask = User_Task.idTask 
                INNER JOIN Users ON User_Task.idUser = Users.idUser 
                WHERE Tasks.idProject = @idProject
                ORDER BY Tasks.deadline ASC";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProject", projectId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int taskId = Convert.ToInt32(reader["idTask"]);
                            string taskName = reader["nameTask"].ToString();
                            string taskDescription = reader["discription"].ToString();
                            DateTime taskDeadline = DateTime.FromOADate(Convert.ToDouble(reader["deadline"]));
                            string taskAssigneeLogin = reader["login"].ToString();
                            bool isChecked = Convert.ToBoolean(reader["status"]);
                            string surname = reader["surname"].ToString();
                            string name = reader["name"].ToString();
							string middlename = reader["middlename"].ToString();
                            string fullName = $"{surname} {name} {middlename}";

                            StackPanel taskPanel = CreateTaskPanel(taskName, taskDescription, taskDeadline, taskAssigneeLogin, isChecked, taskId, projectId, fullName);
                            projectPanel.Children.Add(taskPanel);
                        }
                    }
                }
            }
        }


        public void UpdateTaskStatus(string taskName, bool isChecked)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Tasks SET status = @status WHERE nameTask = @nameTask";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@status", isChecked ? 1 : 0);
                    command.Parameters.AddWithValue("@nameTask", taskName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTaskWindowHandler(StackPanel projectPanel, int projectId)
        {
            CreateTaskWindow createTaskWindow = new CreateTaskWindow(projectId);
            if (createTaskWindow.ShowDialog() == true)
            {
                string taskName = createTaskWindow.TaskName;
                string taskDescription = createTaskWindow.TaskDescription;
                DateTime taskDeadline = createTaskWindow.TaskDeadline;
                int taskAssigneeId = createTaskWindow.TaskAssigneeId;
                bool isChecked = false;

                SaveTaskToDatabase(taskName, taskDescription, taskDeadline, projectId, taskAssigneeId, isChecked);

                string taskAssigneeLogin = GetUserLogin(taskAssigneeId);
                int taskId = GetLastInsertedTaskId(); 
                StackPanel taskPanel = CreateTaskPanel(taskName, taskDescription, taskDeadline, taskAssigneeLogin, isChecked, taskId, projectId, "");
                projectPanel.Children.Add(taskPanel);
            }
        }


        private int GetLastInsertedTaskId()
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT last_insert_rowid()";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private long SaveProjectToDatabase(string projectName, string projectDescription, DateTime projectDeadline, List<int> selectedUserIds)
        {    
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertProjectQuery = "INSERT INTO Projects (nameProject, discription, deadline) VALUES (@nameProject, @discription, @deadline);";
                using (SQLiteCommand command = new SQLiteCommand(insertProjectQuery, connection))
                {
                    command.Parameters.AddWithValue("@nameProject", projectName);
                    command.Parameters.AddWithValue("@discription", projectDescription);
                    command.Parameters.AddWithValue("@deadline", projectDeadline.ToOADate());
                    command.ExecuteNonQuery();
                }

                long projectId = connection.LastInsertRowId;

                foreach (int userId in selectedUserIds)
                {
                    string insertUserProjectQuery = "INSERT INTO User_Project (idUser, idProject) VALUES (@idUser, @idProject);";
                    using (SQLiteCommand command = new SQLiteCommand(insertUserProjectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idUser", userId);
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }
                }

                return projectId;
            }
        }



        private void SaveTaskToDatabase(string taskName, string taskDescription, DateTime taskDeadline, long projectId, int assigneeId, bool isChecked)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertTaskQuery = "INSERT INTO Tasks (nameTask, discription, deadline, status, idProject) VALUES (@nameTask, @discription, @deadline, @status, @idProject);";
                using (SQLiteCommand command = new SQLiteCommand(insertTaskQuery, connection))
                {
                    command.Parameters.AddWithValue("@nameTask", taskName);
                    command.Parameters.AddWithValue("@discription", taskDescription);
                    command.Parameters.AddWithValue("@deadline", taskDeadline.ToOADate());
                    command.Parameters.AddWithValue("@status", isChecked ? 1 : 0);
                    command.Parameters.AddWithValue("@idProject", projectId);
                    command.ExecuteNonQuery();
                }

                long taskId = connection.LastInsertRowId;

                string insertUserTaskQuery = "INSERT INTO User_Task (idUser, idTask) VALUES (@idUser, @idTask);";
                using (SQLiteCommand command = new SQLiteCommand(insertUserTaskQuery, connection))
                {
                    command.Parameters.AddWithValue("@idUser", assigneeId);
                    command.Parameters.AddWithValue("@idTask", taskId);
                    command.ExecuteNonQuery();
                }

                ReloadUserProjects();
            }
        }

        private string GetUserLogin(int userId)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT login FROM Users WHERE idUser = @idUser";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idUser", userId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["login"].ToString();
                        }
                    }
                }
            }
            return string.Empty;
        }

        private void OpenEditProjectWindow(int projectId, TextBlock name, TextBlock description, TextBlock date)
        {
            EditProjectWin editProjectWin = new EditProjectWin(projectId);
            editProjectWin.Owner = this;
            editProjectWin.ProjectNameBox.Text = name.Text;
            editProjectWin.ProjectDescriptionBox.Text = description.Text;
            editProjectWin.DeadlinePicker.SelectedDate = DateTime.Parse(date.Text);

            // Загружаем всех участников проекта и выделяем выбранных
            List<int> selectedUserIds = LoadProjectUsers(projectId);
            editProjectWin.LoadUsersAndSelectParticipants(selectedUserIds);

            if (editProjectWin.ShowDialog() == true)
            {
                string updatedName = editProjectWin.ProjectNameBox.Text;
                string updatedDescription = editProjectWin.ProjectDescriptionBox.Text;
                DateTime updatedDeadline = editProjectWin.DeadlinePicker.SelectedDate.Value;
                List<int> updatedUserIds = editProjectWin.UsersListBox.SelectedItems.Cast<User>().Select(u => u.IdUser).ToList();

                UpdateProjectInDatabase(projectId, updatedName, updatedDescription, updatedDeadline, updatedUserIds);

                // Обновление отображения проекта на доске
                name.Text = updatedName;
                description.Text = updatedDescription;
                date.Text = updatedDeadline.ToString();

                ReloadUserProjects();
            }
        }

        private List<int> LoadProjectUsers(int projectId)
        {
            List<int> selectedUserIds = new List<int>();
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT idUser FROM User_Project WHERE idProject = @idProject";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@idProject", projectId);
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            selectedUserIds.Add(Convert.ToInt32(reader["idUser"]));
                        }
                    }
                }
            }
            return selectedUserIds;
        }

        private void UpdateProjectInDatabase(int projectId, string name, string description, DateTime deadline, List<int> userIds)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    // Обновляем проект
                    string updateProjectQuery = @"
                    UPDATE Projects
                    SET nameProject = @nameProject,
                        discription = @discription,
                        deadline = @deadline
                    WHERE idProject = @idProject";
                    using (SQLiteCommand command = new SQLiteCommand(updateProjectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@nameProject", name);
                        command.Parameters.AddWithValue("@discription", description);
                        command.Parameters.AddWithValue("@deadline", deadline.ToOADate());
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    // Удаляем старых участников проекта
                    string deleteUserProjectQuery = "DELETE FROM User_Project WHERE idProject = @idProject";
                    using (SQLiteCommand command = new SQLiteCommand(deleteUserProjectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idProject", projectId);
                        command.ExecuteNonQuery();
                    }

                    // Добавляем новых участников проекта
                    string insertUserProjectQuery = "INSERT INTO User_Project (idUser, idProject) VALUES (@idUser, @idProject)";
                    foreach (int userId in userIds)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(insertUserProjectQuery, connection))
                        {
                            command.Parameters.AddWithValue("@idUser", userId);
                            command.Parameters.AddWithValue("@idProject", projectId);
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        private void OpenEditTaskWindow(int taskId, TextBlock task, TextBlock descriptionTask, TextBlock dateTask, TextBlock assignee, CheckBox taskStatus, int projectId, string fullName)
        {
            EditTaskWin editTaskWin = new EditTaskWin(projectId, taskId, fullName);
            editTaskWin.Owner = this;

            // Устанавливаем текстовые значения полей
            editTaskWin.TaskNameBox.Text = task.Text.Trim();
            editTaskWin.TaskDescriptionBox.Text = descriptionTask.Text.Trim();
            editTaskWin.TaskDeadlinePicker.SelectedDate = DateTime.Parse(dateTask.Text);

            // Загружаем исполнителей проекта и устанавливаем выбранного исполнителя
            editTaskWin.LoadAssignees(projectId);

            // Устанавливаем текущего исполнителя задачи
            string currentAssignee = assignee.Text.Split(':')[1].Trim();
            User selectedUser = editTaskWin.AssigneeComboBox.Items
                .OfType<User>()
                .FirstOrDefault(user => $"{user.Surname} {user.Name} {user.Middlename}" == currentAssignee);

            if (selectedUser != null)
            {
                editTaskWin.AssigneeComboBox.SelectedItem = selectedUser;
            }

            if (editTaskWin.ShowDialog() == true)
            {
                string updatedTaskName = editTaskWin.TaskNameBox.Text;
                string updatedTaskDescription = editTaskWin.TaskDescriptionBox.Text;
                DateTime updatedTaskDeadline = editTaskWin.TaskDeadlinePicker.SelectedDate.Value;
                int updatedAssigneeId = (editTaskWin.AssigneeComboBox.SelectedItem as User).IdUser;

                UpdateTaskInDatabase(taskId, updatedTaskName, updatedTaskDescription, updatedTaskDeadline, updatedAssigneeId);

                // Обновление отображения задачи на доске
                task.Text = updatedTaskName + " ";
                descriptionTask.Text = updatedTaskDescription + " ";
                dateTask.Text = updatedTaskDeadline.ToString();
                assignee.Text = $"Исполнитель: {editTaskWin.AssigneeComboBox.SelectedItem}";
            }
            ReloadUserProjects();
        }


        public void ReloadUserProjects()
        {
            ProjectsPanel.Children.Clear(); // Удаляем все проекты из панели
            LoadUserProjects(); 
        }

        public void LoadUserProjects()
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                SELECT Projects.* 
                FROM Projects 
                INNER JOIN User_Project ON Projects.idProject = User_Project.idProject 
                INNER JOIN Users ON User_Project.idUser = Users.idUser 
                WHERE Users.login = @login
                ORDER BY Projects.deadline ASC";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", CurrentUserLogin);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int projectId = Convert.ToInt32(reader["idProject"]);
                            string projectName = reader["nameProject"].ToString();
                            string projectDescription = reader["discription"].ToString();
                            DateTime projectDeadline = DateTime.FromOADate(Convert.ToDouble(reader["deadline"]));

                            LoadProject(projectName, projectDescription, projectDeadline, null, projectId);
                        }
                    }
                }
            }
        }

        private void UpdateTaskInDatabase(int taskId, string taskName, string taskDescription, DateTime taskDeadline, int assigneeId)
        {
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    // Обновляем задачу в таблице Tasks
                    string updateTaskQuery = @"
                    UPDATE Tasks
                    SET nameTask = @nameTask,
                        discription = @discription,
                        deadline = @deadline
                    WHERE idTask = @idTask";
                    using (SQLiteCommand command = new SQLiteCommand(updateTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@nameTask", taskName);
                        command.Parameters.AddWithValue("@discription", taskDescription);
                        command.Parameters.AddWithValue("@deadline", taskDeadline.ToOADate());
                        command.Parameters.AddWithValue("@idTask", taskId);
                        command.ExecuteNonQuery();
                    }
                    // Обновляем исполнителя задачи в таблице User_Task
                    string updateUserTaskQuery = @"
                    UPDATE User_Task
                    SET idUser = @idUser
                    WHERE idTask = @idTask";
                    using (SQLiteCommand command = new SQLiteCommand(updateUserTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idUser", assigneeId);
                        command.Parameters.AddWithValue("@idTask", taskId);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void personalButton_Click(object sender, RoutedEventArgs e)
        {
            PersonalWin personalWin = new PersonalWin(CurrentUserLogin);
            personalWin.Show();
            this.Close();
        }
    }
}