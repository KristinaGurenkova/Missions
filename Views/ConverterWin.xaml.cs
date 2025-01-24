using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ConverterTasks;
using Missions.Classes;
using static System.Net.Mime.MediaTypeNames;
using Task = Missions.Classes.Task;

namespace Missions.Views
{
    /// <summary>
    /// Логика взаимодействия для ConverterWin.xaml
    /// </summary>
    public partial class ConverterWin : Window
    {
        public string CurrentUserLogin { get; }
        Converter converter = new Converter();
        List<Tasks> TasksList= new List<Tasks>();
        public ConverterWin(string login)
        {
            InitializeComponent();
            CurrentUserLogin = login;
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
                            Tasks task = new Tasks
                            {
                                IdTasks = Convert.ToInt32(reader["idTask"]),
                                NameTasks = reader["nameTask"].ToString(),
                                DiscTasks = reader["discription"].ToString(),
                                DeadlineTasks = DateTime.FromOADate(Convert.ToDouble(reader["deadline"])),
                                StatusTasks = Convert.ToBoolean(reader["status"]),
                            };
                            TasksList.Add(task);
                        }
                    }
                }
            }
        }
        private void txtButton_Click(object sender, RoutedEventArgs e)
        {

           converter.txtConvert(TasksList);
            
        }
        private void jsonButton_Click(object sender, RoutedEventArgs e)
        {
			converter.jsonConvert(TasksList);
		}
    }
}