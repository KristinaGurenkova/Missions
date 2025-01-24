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
    /// Логика взаимодействия для PersonalWin.xaml
    /// </summary>
    public partial class PersonalWin : Window
    {
        static public string CurrentUserLogin { get; set; }

        public PersonalWin(string userLogin)
        {
            InitializeComponent();
            logBox.Text = userLogin;
            CurrentUserLogin = userLogin;
            LoadProjectTasks();
        }

        private void LoadProjectTasks()
        {
            string query;
            BoardWin boardWin = new BoardWin(CurrentUserLogin);
            string connectionString = "Data Source=Missions.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                query = @"
                SELECT Tasks.*, Users.login 
                FROM Tasks 
                INNER JOIN User_Task ON Tasks.idTask = User_Task.idTask 
                INNER JOIN Users ON User_Task.idUser = Users.idUser
                WHERE Users.login = @login
                ORDER BY Tasks.deadline ASC";
                
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", CurrentUserLogin);
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

                            StackPanel taskPanel = CreateTaskPanel(taskName, taskDescription, taskDeadline, taskAssigneeLogin, isChecked, taskId);
                        }
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
        private StackPanel CreateTaskPanel(string taskName, string taskDescription, DateTime taskDeadline, string taskAssigneeLogin, bool isChecked, int taskId)
        {
            BoardWin boardWin = new BoardWin(CurrentUserLogin);
            Border projectBorder = new Border
            {
                Style = (Style)Application.Current.Resources["RoundedBorderStyle"],
                Margin = new Thickness(10, 10, 10, 10)
            };
            StackPanel taskPanel = new StackPanel { Orientation = Orientation.Vertical };
            projectBorder.Child = taskPanel;

            StackPanel taskNamePanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock task = new TextBlock()
            {
                Text = "Задача: ",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            TextBlock taskNameText = new TextBlock()
            {
                Text = taskName,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };
            taskNamePanel.Children.Add(task);
            taskNamePanel.Children.Add(taskNameText);

            StackPanel descriptionTaskPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock description = new TextBlock()
            {
                Text = "Описание: ",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            TextBlock descriptionTaskText = new TextBlock()
            {
                Text = taskDescription,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };
            descriptionTaskPanel.Children.Add(description);
            descriptionTaskPanel.Children.Add(descriptionTaskText);

            StackPanel assigneePanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock assignee = new TextBlock()
            {
                Text = "Исполнитель: ",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            TextBlock assigneeText = new TextBlock()
            {
                Text = taskAssigneeLogin,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };
            assigneePanel.Children.Add(assignee);
            assigneePanel.Children.Add(assigneeText);

            StackPanel dateTaskPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock date = new TextBlock()
            {
                Text = "Дедлайн: ",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            TextBlock dateTaskText = new TextBlock()
            {
                Text = taskDeadline.ToString(),
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };
            dateTaskPanel.Children.Add(date);
            dateTaskPanel.Children.Add(dateTaskText);

            StackPanel statusPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock statusText = new TextBlock()
            {
                Text = "Выполнено: ",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            CheckBox taskStatus = new CheckBox
            {
                IsEnabled = taskAssigneeLogin == CurrentUserLogin,
                IsChecked = isChecked,
                Margin = new Thickness(0, 6, 6, 0),
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            taskStatus.Checked += (s, e) => boardWin.UpdateTaskStatus(taskName, true);
            taskStatus.Unchecked += (s, e) => boardWin.UpdateTaskStatus(taskName, false);
            statusPanel.Children.Add(statusText);
            statusPanel.Children.Add(taskStatus);

            taskPanel.Children.Add(statusPanel);
            taskPanel.Children.Add(taskNamePanel);
            taskPanel.Children.Add(descriptionTaskPanel);
            taskPanel.Children.Add(dateTaskPanel);

            TasksPanel.Children.Add(projectBorder);

            return taskPanel;
        }


        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            BoardWin boardWin = new BoardWin(CurrentUserLogin);
            this.Close();
            boardWin.Show();
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            ConverterWin converterWin = new ConverterWin(CurrentUserLogin);
            converterWin.ShowDialog();
        }
    }
}
