/* 
 * Created by Andrew Maney
 * Date: 2/16/2022
 * Time: 4:46 PM
 * .NET Version: 4.0
 * License: MIT
 * 
 */ 
 
 
// Required Libraries
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;


// Main Program
namespace Portable_Minecraft_Launcher
{
	// Secondary Class for initialization
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Portable Minecraft Launcher\nWritten By: Andrew Maney, 2022");
			Console.WriteLine("-----------------------------------------------------------\n");
			var MainPG = new MainProgram();
			MainPG.MAIN();
		}		
	}
	
	
	// Main class for the launcher
	class MainProgram
	{
		// Variables
		public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public string mc_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft";
		public string LauncherURL = "https://launcher.mojang.com/download/Minecraft.exe";
		public int ExitTime = 5000;
		
		
		// Functions
		// Check if any installations exist before proceeding
		public bool CheckForInstallations(string path)
		{
			Console.WriteLine("[INFO] >> Checking for existing installations (In path \"{0}\")...", path);			
			if(Directory.Exists(mc_path))
			{
				Console.WriteLine("[WARNING] >> Minecraft path \"{0}\" Already exists!", path);
				return true;
			}
			else
			{
				Console.WriteLine("[INFO] >> No installations were found.");
				return false;
			}
		}
		
		
		// Make the required directories
		public void MakeDirs()
		{
			Console.Write("[INFO] >> Making Directories... ");
			if(!Directory.Exists("./mcdata"))
			{				
				Directory.CreateDirectory("./mcdata");
			}
			else
			{
				Console.WriteLine("\n[WARNING] >> Directory \"./mcdata\" Already exists!");
			}
			
			if(!Directory.Exists("./bin"))
			{
				Directory.CreateDirectory("./bin");
			}
			else
			{
				Console.WriteLine("[WARNING] >> Directory \"./bin\" Already exists!");
			}
			
			if(!Directory.Exists("./mcdata/.minecraft"))
			{
				Directory.CreateDirectory("./mcdata/.minecraft");
			}
			else
			{
				Console.WriteLine("[WARNING] >> Directory \"./mcdata/.minecraft\" Already exists!");
			}
			Console.WriteLine("[DONE]\n");
		}
		
		
		// Run the launcher
		public void RunLauncher()
		{
			Console.Write("[INFO] >> Running Launcher... ");
			ProcessStartInfo startInfo = new ProcessStartInfo();
	        startInfo.CreateNoWindow = false;
	        startInfo.UseShellExecute = false;
	        startInfo.FileName = "./bin/Minecraft.exe";
	        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
	        startInfo.Arguments = "--workDir " + Directory.GetCurrentDirectory() + "/mcdata/.minecraft";
	        try
	        {
	            using (Process exeProcess = Process.Start(startInfo))
	            {
	                exeProcess.WaitForExit();
	            }
	        }
	        catch(Exception EX)
	        {
	            Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...",EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
	        }
	        Console.WriteLine("[DONE]\n");
		}
		
		
		// Download the launcher
		public void DownloadLauncher()
		{
			Console.Write("[INFO] >> Downloading Launcher... ({0})", LauncherURL);
			try
			{
				if(File.Exists("./bin/Minecraft.exe") == true)
				{
					Console.WriteLine("[WARNING] >> File \"./bin/Minecraft.exe\" Already exists!");
				}
				else
				{
					using (var client = new WebClient())
					{
	    				client.DownloadFile(LauncherURL, "./bin/Minecraft.exe");
					}				
				}
				Console.WriteLine(" [DONE]");
			}
			catch(Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...",EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);	
			}
		}
		
		
		// Main Function
		public void MAIN()
		{
			if(CheckForInstallations(mc_path) == false)
			{
				MakeDirs();
				DownloadLauncher();
				RunLauncher();
			}
			else
			{
				Console.WriteLine("Continue Anyway?");
				Console.WriteLine("\n1: Yes\n2: No\n");
				Console.Write("Choose an option >> ");
				if(Console.ReadLine() == "1")
				{
					Console.Write("\n");
					MakeDirs();
					DownloadLauncher();
					RunLauncher();
				}
				else
				{
					Console.WriteLine("[WARNING] >> Operation Aborted. Exitting in {0} second(s)...", (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);					
				}				
			}
			Console.WriteLine("[INFO] >> Operation completed successfully. Exitting in {0} second(s)...", (ExitTime / 1000));
			Thread.Sleep(ExitTime);
			Environment.Exit(0);
		}
	}
}
