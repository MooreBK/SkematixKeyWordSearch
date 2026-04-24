//using System;
//using System.Collections.Generic;
using System.Diagnostics;
//using System.IO;
//using System.Linq;

namespace FlowchartSearchApp
{
	class Program
	{
		// The Main method is the starting point of the application
		static void Main(string[] args)
		{
			Console.WriteLine("Starting file search...");

			try
			{
				FlowchartParser parser = new FlowchartParser();
				parser.ReadTextFileAndProcess();
				Console.WriteLine("\nProcessing complete! Press any key to exit.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}

			Console.ReadKey(); // Keeps the window open until you press a key
		}
	}

	public class FlowchartParser
	{
		public void ReadTextFileAndProcess()
		{
			Console.Write("Enter filename (Default: C:\\Users\\BrianM\\OneDrive - State of Oklahoma\\Documents\\skematics search\\AllCharts_20190517.txt): ");
			// The input is trimmed to handle cases where the user only enters whitespace.
			string input = Console.ReadLine()?.Trim();
			string filePath = string.IsNullOrEmpty(input) ? "C:\\Users\\BrianM\\OneDrive - State of Oklahoma\\Documents\\skematics search\\AllCharts_20190517.txt" : input;
			//string filePath = defaults to "C:\Users\BrianM\OneDrive - State of Oklahoma\Documents\skematics search\AllCharts_20190517.txt"
			
			Console.Write("Enter keyword to find (i.e. sunsystems): ");
			// The input is trimmed to handle cases where the user only enters whitespace.
			string keyword = Console.ReadLine()?.Trim();
			string lookfor = string.IsNullOrEmpty(keyword) ? "sunsystems" : keyword;
			//string lookfor = defaults to "sunsystems"
			
			
			if (!File.Exists(filePath))
			{
				Console.WriteLine($"Error: File not found at {filePath}");
				return;
			}

			List<string[]> outputData = new List<string[]>();
			string header1 = "FlowChartName";
			string header2 = "FlowChartData";
			outputData.Add(new string[] { header1, header2 });
			string currentFlowchartName = "";
			bool foundFirstSS = false;

			// 'using' ensures the file is closed automatically
			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Contains("<flowchart name", StringComparison.OrdinalIgnoreCase))
					{
						currentFlowchartName = line.Trim();
						foundFirstSS = false;

						while ((line = reader.ReadLine()) != null && !line.Contains("</flowchart"))
						{
							if (line.Contains(lookfor, StringComparison.OrdinalIgnoreCase))
							{
								if (!foundFirstSS)
								{
									outputData.Add(new string[] { currentFlowchartName, "" });
									foundFirstSS = true;
								}
								outputData.Add(new string[] { "", line.Trim() });
							}
						}
					}
				}
			}

			ExportToCsv(outputData, "C:\\Users\\BrianM\\OneDrive - State of Oklahoma\\Documents\\Search_Results.csv");
		}

		private void ExportToCsv(List<string[]> data, string fileName)
		{
			// Converts the list of arrays into comma-separated lines for Excel
			var lines = data.Select(row => string.Join(",", row.Select(cell => $"\"{cell}\"")));
			File.WriteAllLines(fileName, lines);

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = fileName,
				UseShellExecute = true
			};
			Process.Start(startInfo);

			Console.WriteLine($"Results saved to: {Path.GetFullPath(fileName)}");
		}
	}
}