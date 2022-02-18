/* 
 * Written by Andrew Maney
 * Date: 2/18/2022
 * Time: 5:45 PM
 * .NET Version: 4.0
 * License: MIT
 * 
 */ 
 
 
// Required Libraries
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;


// Main Program
namespace Portable_Minecraft_Launcher
{
	// Class used for initialization
	class Program
	{
		// Variables
		public static string Version = "2-22_b";
		
		
		// Functions
		public static void Main(string[] args)
		{
			Console.WriteLine("Portable Minecraft Launcher\nWritten By: Andrew Maney, 2022\nVersion: {0}", Version);
			Console.WriteLine("-----------------------------------------------------------\n");
			var MainPG = new MainProgram();
			MainPG.MAIN();
		}		
	}
	
	
	// Main class used for downloading/running the launcher
	class MainProgram
	{
		// Variables
		public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public string mc_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft";
		public string LauncherURL = "https://launcher.mojang.com/download/Minecraft.exe";
		public string BackupLauncherURL = "https://drive.google.com/u/1/uc?id=1L6PQpcQCLFv1sVZbbqQwfPrU1gOoAq2Y&export=download&confirm=t";
		public string LauncherDataZIP = "https://drive.google.com/u/1/uc?id=1bw026YbaEr_uHKIFrj04aTd6UPRqclVf&export=download&confirm=t";
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
			try
			{
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
			catch(Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...",EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
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
			Console.Write("[INFO] >> Downloading Launcher... ({0}) ", LauncherURL);
			try
			{
				if(File.Exists("./bin/Minecraft.exe") == true)
				{
					Console.WriteLine("\n[WARNING] >> File \"./bin/Minecraft.exe\" Already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
    						client.DownloadFile(LauncherURL, "./bin/Minecraft.exe");
						}
						Console.WriteLine("[DONE]");
					}
					catch
					{
						Console.Write("\n[WARNING] >> Default launcher URL is unreachable! Attempting to use a backup URL... ({0})", BackupLauncherURL);
						try
						{
							using (var client = new WebClient())
							{
	    						client.DownloadFile(BackupLauncherURL, "./bin/Minecraft.exe");
							}							
							Console.Write(" [DONE]\n[INFO] >> Downloading Launcher Data... ({0})", LauncherDataZIP);
							using (var client = new WebClient())
							{
	    						client.DownloadFile(LauncherDataZIP, "./bin/game_data.zip");
							}							
							Console.Write(" [DONE]\n[INFO] >> Unzipping data...");
							var startInfo = new ProcessStartInfo()
						    {
						        FileName = "powershell.exe",
						        Arguments = "-NoProfile -ExecutionPolicy unrestricted -command \"Expand-Archive -Force ./bin/game_data.zip ./bin\"",
						        UseShellExecute = false,
						        
						    };
							Process p = Process.Start(startInfo);
							p.WaitForExit();
							Console.Write(" [DONE]\n[INFO] >> Removing ZIP...");
							File.Delete("./bin/game_data.zip");
							Console.Write(" [DONE]\n\n");							
						}
						catch(Exception EX)
						{							
							Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
							Thread.Sleep(ExitTime);
							Environment.Exit(1);	
						}
					}
													
				}
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
			try
			{
				if(CheckForInstallations(mc_path) == false)
				{
					MakeDirs();
					DownloadLauncher();
					RunLauncher();
				}
				else
				{
					Console.WriteLine("\n!!! Continue Anyway? !!!");
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
				Console.WriteLine("[INFO] >> Operation completed. Exitting in {0} second(s)...", (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(0);
			}
			catch(Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...",EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
		}
	}
}
