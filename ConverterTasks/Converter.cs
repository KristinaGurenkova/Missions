using System.Text.Json;
using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Text;

namespace ConverterTasks
{
	public class Converter
	{
		public void txtConvert(List<Tasks> tasks, string path)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(path))
				{
					foreach (Tasks task in tasks)
					{
						string text = $"Наименование: {task.NameTasks}\nОписание: {task.DiscTasks}\nДедлайн: {task.DeadlineTasks}\nСтатус выполнения: {(task.StatusTasks ? "Выполнено" : "Не выполнено")}";
						writer.WriteLine(text);
					}
				}
			}
			catch { }
		}
		public void jsonConvert(List<Tasks> tasks, string path)
		{
			try
			{
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};
				string jsonString = JsonSerializer.Serialize(tasks, options);

				using (StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(true)))
				{
					writer.Write(jsonString);
				}
			}
			catch { }
		}
	}
	public class Tasks
	{
		public int IdTasks { get; set; }
		public string NameTasks { get; set; }
		public string DiscTasks { get; set; }
		public DateTime DeadlineTasks { get; set; }
		public bool StatusTasks { get; set; }
	}
}
