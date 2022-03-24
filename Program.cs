/* 
 * Written by Andrew Maney
 * Date: 3/24/2022
 * Time: 10:20 AM
 * .NET Version: 3.1
 * License: MIT
 *
 */


// Required Libraries
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;


// Main Program
namespace PortableMinecraftLauncher
{
	// Class used for initialization
	class Program
	{
		// Variables
		public static string Version = "3-22_1.3_b";

		// Functions
		public static void Main(string[] args)
		{
			Console.WriteLine("-----------------------------------------------------------");
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
		string JRE = "https://drive.google.com/u/2/uc?id=16UlNsq_Id7WV_cOfVOzW7Zea1UMfPqan&export=download&confirm=t";
		public string LauncherDataZIP = "https://drive.google.com/u/1/uc?id=1bw026YbaEr_uHKIFrj04aTd6UPRqclVf&export=download&confirm=t";
		public string VersionURL = "https://raw.githubusercontent.com/MEMESCOEP/PortableMinecraftLauncher/main/Version";
		public string ReleasesURL = "https://github.com/MEMESCOEP/PortableMinecraftLauncher/releases/latest";
		public string PyLauncherURL = "https://anhydrous-newfoundland-4986.dataplicity.io/MC_Launcher.exe";
		public string GameURL = "https://drive.google.com/u/2/uc?id=1YfAa5gZ2gnm7IA0AEtJyZTB0-wEqo-cY&export=download&confirm=t";
		public string OptifineURL = "https://anhydrous-newfoundland-4986.dataplicity.io/files/Optifine.jar";
		public int ExitTime = 5000;
		public string VersionWEB = "";
		public string Version = "3-22_1.3_b";


		// Functions
		// Check if any installations exist before proceeding
		public bool CheckForInstallations(string path)
		{
			Console.WriteLine("[INFO] >> Checking for existing installations (In path \"{0}\")...", path);
			if (Directory.Exists(mc_path))
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


		// Check for updates
		public void CheckForUpdates(string URL, string ver)
		{
			try
			{
				Console.WriteLine("[INFO] >> Checking for updates...");
				using (WebClient client = new WebClient())
				{
					ServicePointManager.Expect100Continue = true;
					ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
					client.Headers.Add("user-agent", "Anything");
					VersionWEB = client.DownloadString(URL).Replace("\n", "");
				}
				Console.WriteLine("[INFO] >> Got Version \"" + VersionWEB + "\"");
				if (VersionWEB != ver)
				{
					Console.WriteLine("A new version is avaliable! Would you like to download it now?");
					Console.WriteLine("\n1: Yes\n2: No\n");
					Console.Write("Choose an option >> ");
					if (Console.ReadLine() == "1")
					{
						Process.Start(ReleasesURL);
					}
					Console.Clear();
				}
				else
				{
					Console.WriteLine("[INFO] >> No new versions found.\n");
				}
				Console.WriteLine("");
			}
			catch (Exception EX)
			{
				Console.WriteLine("[ERORR] >> Failed to get version information: " + EX.Message);
			}
		}


		// Make the required directories (If they don't exist)
		public void MakeDirs()
		{
			Console.WriteLine("[INFO] >> Making Directories... ");
			try
			{
				if (!Directory.Exists("./mcdata"))
				{
					Console.WriteLine("[INFO] >> Creating Directory \"./mcdata\"... ");
					Directory.CreateDirectory("./mcdata");
				}
				else
				{
					Console.WriteLine("\n[WARNING] >> Directory \"./mcdata\" Already exists!");
				}

				if (!Directory.Exists("./bin"))
				{
					Console.WriteLine("[INFO] >> Creating Directory \"./bin\"... ");
					Directory.CreateDirectory("./bin");
				}
				else
				{
					Console.WriteLine("[WARNING] >> Directory \"./bin\" Already exists!");
				}

				if (!Directory.Exists("./mcdata/.minecraft"))
				{
					Console.WriteLine("[INFO] >> Creating Directory \"./mcdata/.minecraft\"... ");
					Directory.CreateDirectory("./mcdata/.minecraft");
				}
				else
				{
					Console.WriteLine("[WARNING] >> Directory \"./mcdata/.minecraft\" Already exists!");
				}

				if (!Directory.Exists("./bin/JRE"))
				{
					Console.WriteLine("[INFO] >> Creating Directory \"./bin/JRE\"... ");
					Directory.CreateDirectory("./bin/JRE");
				}
				else
				{
					Console.WriteLine("[WARNING] >> Directory \"./bin/JRE\" Already exists!");
				}
				Console.WriteLine("[DONE]\n\n");
			}
			catch (Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
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
			catch (Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
			Console.WriteLine("[DONE]\n\n");
		}


		// Download MC 1.18.2
		public void Download_MC()
		{
			Console.Write("[INFO] >> Downloading Minecraft... ({0}) ", GameURL);
			try
			{
				if (Directory.Exists("./mcdata/.minecraft/assets") == true)
				{
					Console.WriteLine("\n[WARNING] >> Directory \"./mcdata/.minecraft/assets\" Already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
							client.Headers.Add("User-Agent: Other");
							client.DownloadFile(GameURL, "./mcdata/.minecraft/GAME.zip");
						}
						Console.WriteLine("[DONE]");
						Console.Write(" [DONE]\n\n[INFO] >> Unzipping data...");
						ZipFile.ExtractToDirectory("./mcdata/.minecraft/GAME.zip", "./mcdata/.minecraft/");

						Console.Write(" [DONE]\n\n[INFO] >> Removing ZIP...");
						File.Delete("./mcdata/.minecraft/GAME.zip");
						Console.Write(" [DONE]\n\n");
					}
					catch (Exception EX)
					{
						Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
						Thread.Sleep(ExitTime);
						Environment.Exit(1);
					}

				}
			}
			catch (Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
		}


		// Run the game with the JRE
		public void RunGame()
		{
			Console.Write("\n\n\n\n\nUsername >> ");
			string Username = Console.ReadLine();
			Console.WriteLine("[INFO] >> Starting game...");
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.CreateNoWindow = false;
				startInfo.UseShellExecute = false;
				startInfo.FileName = "./MC_Launcher.exe";
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.Arguments = Username;
				try
				{
					using (Process exeProcess = Process.Start(startInfo))
					{
						exeProcess.WaitForExit();
					}
				}
				catch (Exception EX)
				{
					Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);
				}
				Console.WriteLine("[DONE]\n\n");
			}
			catch
			{

			}
			try
			{
				Console.Write("[INFO] >> Getting access token...");
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.CreateNoWindow = false;
				startInfo.UseShellExecute = false;
				startInfo.FileName = "powershell.exe";
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.Arguments = "-command \"Get-WmiObject -Query \\\"SELECT CommandLine FROM Win32_Process WHERE Name LIKE \\\"\\\" % Java % \\\"\\\" AND CommandLine LIKE \\\"\\\" % accessToken % \\\"\\\"\\\" | Select-Object -Property CommandLine | Out-String -width 9999 | Out-File \\\"TOKEN.txt\\\"\"";
				try
				{
					using (Process exeProcess = Process.Start(startInfo))
					{
						exeProcess.WaitForExit();
					}
				}
				catch (Exception EX)
				{
					Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);
				}
				Console.WriteLine("[DONE]\n\n");
			}
			catch
			{

			}
		}


		// Download Optifine 1.18.2
		public void DownloadOptifine()
		{
			Console.Write("[INFO] >> Downloading Optifine... ({0}) ", OptifineURL);
			try
			{
				if (File.Exists("./bin/Optifine.jar") == true)
				{
					Console.WriteLine("\n[WARNING] >> File \"./bin/Optifine.jar\" Already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
							client.Headers.Add("User-Agent: Other");
							client.DownloadFile(OptifineURL, "./bin/Optifine.jar");
						}
						Console.WriteLine("[DONE]");
					}
					catch(Exception EX)
					{
						Console.WriteLine("[ERROR] >> Failed to download Optifine.jar! Details: " + EX.Message);
					}
				}
			}
			catch
			{

			}
		}

		// Download the launcher
		public void DownloadLauncher()
		{
			Console.Write("[INFO] >> Downloading Launcher... ({0}) ", LauncherURL);
			try
			{
				if (File.Exists("./bin/Minecraft.exe") == true)
				{
					Console.WriteLine("\n[WARNING] >> File \"./bin/Minecraft.exe\" Already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
							client.Headers.Add("User-Agent: Other");
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
								client.Headers.Add("User-Agent: Other");
								client.DownloadFile(BackupLauncherURL, "./bin/Minecraft.exe");
							}
							Console.Write(" [DONE]\n\n[INFO] >> Downloading Launcher Data... ({0})", LauncherDataZIP);
							using (var client = new WebClient())
							{
								client.Headers.Add("User-Agent: Other");
								client.DownloadFile(LauncherDataZIP, "./bin/game_data.zip");
							}
							Console.Write(" [DONE]\n\n[INFO] >> Unzipping data...");
							ZipFile.ExtractToDirectory("./bin/game_data.zip", "./bin");

							Console.Write(" [DONE]\n\n[INFO] >> Removing ZIP...");
							File.Delete("./bin/game_data.zip");
							Console.Write(" [DONE]\n\n");
						}
						catch (Exception EX)
						{
							Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
							Thread.Sleep(ExitTime);
							Environment.Exit(1);
						}
					}

				}
			}
			catch (Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
		}


		// Download the custom launcher
		public void Download_PY_Launcher()
		{
			Console.Write("[INFO] >> Downloading Python Launcher... ({0}) ", PyLauncherURL);
			try
			{
				if (File.Exists("./MC_Launcher.exe") == true)
				{
					Console.WriteLine("\n[WARNING] >> File \"./MC_Launcher.exe\" Already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
							client.Headers.Add("User-Agent: Other");
							client.DownloadFile(PyLauncherURL, "./MC_Launcher.exe");
						}
						Console.WriteLine("[DONE]\n\n");
					}
					catch (Exception EX)
					{
						Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
						Thread.Sleep(ExitTime);
						Environment.Exit(1);
					}

				}
			}
			catch (Exception EX)
			{
				Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
		}

		// Download the JRE
		public void DownloadJRE()
		{
			Console.Write("[INFO] >> Downloading Java Runtime Enviornment... ({0}) ", JRE);
			try
			{
				if (Directory.Exists("./bin/JRE/bin") == true)
				{
					Console.WriteLine("\n[WARNING] >> Directory \"./bin/JRE/bin\" already exists!");
				}
				else
				{
					try
					{
						using (var client = new WebClient())
						{
							client.Headers.Add("User-Agent: Other");
							client.DownloadFile(JRE, "./bin/JRE.zip");
						}
						Console.Write(" [DONE]\n\n[INFO] >> Unzipping JRE...");

						ZipFile.ExtractToDirectory("./bin/JRE.zip", "./bin/JRE");

						Console.Write(" [DONE]\n\n[INFO] >> Removing ZIP...");
						File.Delete("./bin/JRE.zip");
						Console.Write(" [DONE]\n\n");
					}
					catch (Exception EX)
					{
						Console.WriteLine("\n[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
						Thread.Sleep(ExitTime);
						Environment.Exit(1);
					}
				}
			}
			catch (Exception EX)
			{
				Console.WriteLine("\n[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
				Thread.Sleep(ExitTime);
				Environment.Exit(1);
			}
		}

		// Main Function
		public void MAIN()
		{
			Console.Title = "Portable MC Launcher [" + Version + "] - Andrew Maney";
			CheckForUpdates(VersionURL, Version);
			Console.WriteLine("[Choose mode]");
			Console.WriteLine("\n1: Start with official launcher\n2: Start with JRE (Advanced, No ONLINE multiplayer support)\n3: Install Optifine (You must run the official launcher at least once before using this option!)\n4: Exit\n");
			Console.Write("Choose an option >> ");
			string response = Console.ReadLine();
			if (response == "1")
			{
				try
				{
					if (CheckForInstallations(mc_path) == false)
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
						if (Console.ReadLine() == "1")
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
				catch (Exception EX)
				{
					Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);
				}
			}
			else if (response == "2")
			{
				try
				{
					if (CheckForInstallations(mc_path) == false)
					{
						MakeDirs();
						DownloadJRE();
						Download_PY_Launcher();
						Download_MC();
						RunGame();
					}
					else
					{
						Console.WriteLine("\n!!! Continue Anyway? !!!");
						Console.WriteLine("\n1: Yes\n2: No\n");
						Console.Write("Choose an option >> ");
						if (Console.ReadLine() == "1")
						{
							Console.Write("\n");
							MakeDirs();
							DownloadJRE();
							Download_PY_Launcher();
							Download_MC();
							RunGame();
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
				catch (Exception EX)
				{
					Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);
				}
			}
			else if(response == "3")
            {
				try
				{
					DownloadOptifine();
					Console.WriteLine("[INFO] >> Starting Optifine installer...");
					try
					{
						string binPATH = Directory.GetCurrentDirectory() + "\\bin";
						ProcessStartInfo startInfo = new ProcessStartInfo();
						startInfo.CreateNoWindow = false;
						startInfo.UseShellExecute = false;
						startInfo.FileName = "./bin/JRE/bin/java.exe";
						startInfo.WindowStyle = ProcessWindowStyle.Hidden;
						startInfo.Arguments = "-jar " + binPATH + "\\Optifine.jar";
						try
						{
							using (Process exeProcess = Process.Start(startInfo))
							{
								exeProcess.WaitForExit();
							}
						}
						catch (Exception EX)
						{
							Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
							Thread.Sleep(ExitTime);
							Environment.Exit(1);
						}
						Console.WriteLine("[DONE]\n\n");
					}
					catch
					{

					}
				}
				catch (Exception EX)
				{
					Console.WriteLine("[ERROR] >> Operation Failed.\nDETAILS: {0}\nExitting in {1} second(s)...", EX.Message, (ExitTime / 1000));
					Thread.Sleep(ExitTime);
					Environment.Exit(1);
				}
			}
			else if (response == "4")
			{
				Environment.Exit(0);
			}
		}
	}
}
