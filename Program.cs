/* 
 * Created by Andrew Maney
 * Date: 2/16/2022
 * Time: 4:46 PM
 * 
 */
using System;
using System.IO;
using System.Threading;

namespace Portable_Minecraft_Launcher
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Portable Minecraft Launcher\nWritten By: Andrew Maney, 2022");
			var MainPG = new MainProgram();
			MainPG.MAIN();
		}
		
		
	}
	
	class MainProgram
	{
		// Variables
		public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public string mc_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft";
		public string LOG = "";
		public string LauncherURL = "";
		public int ExitTime = 5000;
		
		// Functions
		public bool CheckForInstallations()
		{
			Console.WriteLine("Checking for existing installations (In path \"{0}\")...", mc_path);
			LOG += ("Checking for existing installations (In path " + mc_path + "\"{0}\")...");
			
			if(Directory.Exists(mc_path))
			{
				Console.WriteLine("Minecraft path \"{0}\" Already exists!", mc_path);
				return true;
			}
			else
			{
				Console.WriteLine("No installations were found.");
				return false;
			}
		}
		
		// Download the launcher
		public void DownloadLauncher()
		{
			Console.WriteLine("Downloading Launcher... ({0})", );
		}
		
		public void MAIN()
		{
			if(CheckForInstallations() == false)
			{
				DownloadLauncher();
			}
			else
			{
				Console.WriteLine("Operation Aborted. Exitting in {0} second(s)...", (ExitTime / 1000));
				Thread.Sleep(ExitTime);
			}
		}
	}
}