// Required Libraries
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.WinForm;

namespace MC_Launcher_GUI
{
    public partial class Form1 : Form
    {
		// Variables
		public string ConsoleData = "";
		public string PrevConsoleData = "";
		public string Username = "Player";
		public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string mc_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\assets";
        public string LauncherURL = "https://launcher.mojang.com/download/Minecraft.exe";
        public string BackupLauncherURL = "https://drive.google.com/u/1/uc?id=1L6PQpcQCLFv1sVZbbqQwfPrU1gOoAq2Y&export=download&confirm=t";
        public string JRE = "https://drive.google.com/u/2/uc?id=16UlNsq_Id7WV_cOfVOzW7Zea1UMfPqan&export=download&confirm=t";
        public string LauncherDataZIP = "https://anhydrous-newfoundland-4986.dataplicity.io/files/GAME_DATA.zip";
        public string VersionURL = "https://raw.githubusercontent.com/MEMESCOEP/PortableMinecraftLauncher/main/Version";
        public string ReleasesURL = "https://github.com/MEMESCOEP/PortableMinecraftLauncher/releases/latest";
        public string PyLauncherURL = "https://anhydrous-newfoundland-4986.dataplicity.io/MC_Launcher.exe";
        public string GameURL = "https://drive.google.com/u/2/uc?id=1YfAa5gZ2gnm7IA0AEtJyZTB0-wEqo-cY&export=download&confirm=t";
        public string OptifineURL = "https://anhydrous-newfoundland-4986.dataplicity.io/files/Optifine.jar";
        public int ExitTime = 5000;
        public string VersionWEB = "";
        public string Version = "3-22_3.11_RC";
		public string AccessToken = "";
		public bool GameRunning = false;
		public bool CheckedForUpdates = false;
		public bool IsLoggedIn = false;
		public RegistryKey GameKey;


		// Functions
		public Form1()
        {
            InitializeComponent();
            try
            {
				if (CheckedForUpdates == false)
				{
					CheckForUpdates(VersionURL, Version);
					CheckedForUpdates = true;
					GameKey = Registry.CurrentUser.OpenSubKey("UNAME", true);
					if (GameKey != null)
					{
						IsLoggedIn = true;
						Username = (string)GameKey.GetValue("Name");
					}
					else
					{
						GameKey = Registry.CurrentUser.CreateSubKey("UNAME", true);
						GameKey.SetValue("Name", Username);
						GameKey.Close();
					}
				}
			}
            catch(Exception EX)
            {
				MessageBox.Show("ERROR: " + EX.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


		public void ChangeUsername()
        {
            try
			{
				DialogResult DR = ShowInputDialogBox(ref Username, "Enter a username", "Enter a username", 300, 100);
				if (DR == DialogResult.OK)
                {
					GameKey = Registry.CurrentUser.OpenSubKey("UNAME", true);
					GameKey.SetValue("Name", Username);
					GameKey.Close();
				}				
			}
			catch (Exception EX)
			{
				MessageBox.Show("ERROR: " + EX.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void button1_Click(object sender, EventArgs e)
        {
			if (CheckForInstallations(mc_path) == false)
			{
				Thread thread = new Thread(() =>
				{
					MakeDirs();
					DownloadLauncher();
					RunLauncher();
				});
				thread.Start();
				GameRunning = true;
			}
			else
			{
				DialogResult Continue = MessageBox.Show("An installation already exists! Do you still want to continue?", "Installation Exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if(Continue == DialogResult.Yes)
                {
					Thread thread = new Thread(() =>
					{
						MakeDirs();
						DownloadLauncher();
						RunLauncher();
					});
					thread.Start();
					GameRunning = true;
				}
                else
                {
					ConsoleData += ("[WARNING] >> Operation Aborted.");

				}
			}
		}


		// Run the launcher
		public void RunLauncher()
		{
			ConsoleData += ("[INFO] >> Running Launcher...\n");
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
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
				Thread.Sleep(ExitTime);
			}
			ConsoleData += ("[DONE]\n\n");
			GameRunning = false;
		}


		// Download MC 1.18.2
		public void Download_MC()
		{
			ConsoleData += ("[INFO] >> Downloading Minecraft... (" + GameURL + ")\n");
			try
			{
				if (Directory.Exists("./mcdata/.minecraft/assets") == true)
				{
					ConsoleData += ("\n[WARNING] >> Directory \"./mcdata/.minecraft/assets\" Already exists!\n");
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
						ConsoleData += ("[DONE]\n\n[INFO] >> Unzipping data...\n");
						ZipFile.ExtractToDirectory("./mcdata/.minecraft/GAME.zip", "./mcdata/.minecraft/");

						ConsoleData += ("[DONE]\n\n[INFO] >> Removing ZIP...\n");
						File.Delete("./mcdata/.minecraft/GAME.zip");
						ConsoleData += ("[DONE]\n\n");
					}
					catch (Exception EX)
					{
						ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
					}

				}
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
			}
		}


		// Download the custom launcher
		public void Download_PY_Launcher()
		{
			ConsoleData += ("[INFO] >> Downloading Python Launcher... ({0}) ", PyLauncherURL);
			try
			{
				if (File.Exists("./MC_Launcher.exe") == true)
				{
					ConsoleData += ("\n[WARNING] >> File \"./MC_Launcher.exe\" Already exists!");
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
						ConsoleData += ("[DONE]\n\n");
					}
					catch (Exception EX)
					{
						ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + ")\n");
					}

				}
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + ")\n");
			}
		}

		// Download the JRE
		public void DownloadJRE()
		{
			ConsoleData += ("[INFO] >> Downloading Java Runtime Enviornment... (" + JRE + ")\n");
			try
			{
				if (Directory.Exists("./bin/JRE/bin") == true)
				{
					ConsoleData += ("\n[WARNING] >> Directory \"./bin/JRE/bin\" already exists!\n");
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
						ConsoleData += ("[DONE]\n\n[INFO] >> Unzipping JRE...\n");

						ZipFile.ExtractToDirectory("./bin/JRE.zip", "./bin/JRE");

						ConsoleData += ("[DONE]\n\n[INFO] >> Removing ZIP...\n");
						File.Delete("./bin/JRE.zip");
						ConsoleData += ("[DONE]\n\n");
					}
					catch (Exception EX)
					{
						ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
					}
				}
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
			}
		}

		// Run the game with the JRE
		public void RunGame()
		{
			string AccessToken = "";
			ConsoleData += ("[INFO] >> Starting game...");
			GameKey = Registry.CurrentUser.OpenSubKey("UNAME", true);
			if (GameKey != null)
			{
				AccessToken = (string)GameKey.GetValue("AccessToken");
			}

			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.CreateNoWindow = false;
				startInfo.UseShellExecute = false;
				startInfo.FileName = "./MC_Launcher.exe";
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.Arguments = Username + " " + AccessToken;
				try
				{
					using (Process exeProcess = Process.Start(startInfo))
					{
						exeProcess.WaitForExit();
					}
				}
				catch (Exception EX)
				{
					ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
				}
				ConsoleData += ("[DONE]\n\n");
			}
			catch
			{

			}
			GameRunning = false;
			try
			{
				/*
				 * ConsoleData += ("[INFO] >> Getting access token...\n");
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
					ConsoleData += ("[ERROR] >> Operation Failed. DETAILS: " + EX.Message + "\n");
				}
				ConsoleData += ("[DONE]\n\n");
				*/
			}
			catch
			{

			}
		}


		// Download Optifine 1.18.2
		public void DownloadOptifine()
		{
			ConsoleData += ("[INFO] >> Downloading Optifine... (" + OptifineURL + ")...\n");
			try
			{
				if (File.Exists("./bin/Optifine.jar") == true)
				{
					ConsoleData += ("\n[WARNING] >> File \"./bin/Optifine.jar\" Already exists!\n");
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
						ConsoleData += ("[DONE]\n");
					}
					catch (Exception EX)
					{
						ConsoleData += ("[ERROR] >> Failed to download Optifine.jar! Details: " + EX.Message + "\n");
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
			ConsoleData += ("[INFO] >> Downloading Launcher... (" + LauncherURL + ")...\n");
			try
			{
				if (File.Exists("./bin/Minecraft.exe") == true)
				{
					ConsoleData += ("\n[WARNING] >> File \"./bin/Minecraft.exe\" Already exists!\n");
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
						ConsoleData += ("[DONE]\n");
					}
					catch
					{
						ConsoleData += ("\n[WARNING] >> Default launcher URL is unreachable! Attempting to use a backup URL... (" + BackupLauncherURL + ")...\n");
						try
						{
							using (var client = new WebClient())
							{
								client.Headers.Add("User-Agent: Other");
								client.DownloadFile(BackupLauncherURL, "./bin/Minecraft.exe");
							}
							ConsoleData += ("[DONE]\n\n[INFO] >> Downloading Launcher Data... (" + LauncherDataZIP + ")...\n");
							using (var client = new WebClient())
							{
								client.Headers.Add("User-Agent: Other");
								client.DownloadFile(LauncherDataZIP, "./bin/game_data.zip");
							}
							ConsoleData += ("[DONE]\n\n[INFO] >> Unzipping data...\n");
							ZipFile.ExtractToDirectory("./bin/game_data.zip", "./bin");

							ConsoleData += ("[DONE]\n\n[INFO] >> Removing ZIP...\n");
							File.Delete("./bin/game_data.zip");
							ConsoleData += ("[DONE]\n\n");
						}
						catch (Exception EX)
						{
							ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
							//
						}
					}

				}
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
			}
		}


		// Check if any installations exist before proceeding
		public bool CheckForInstallations(string path)
		{
			ConsoleData += ("[INFO] >> Checking for existing installations (In path \"{0}\")...\n", path);
			if (Directory.Exists(mc_path))
			{
				ConsoleData += ("[WARNING] >> Minecraft path \"{0}\" Already exists!\n", path);
				return true;
			}
			else
			{
				ConsoleData += ("[INFO] >> No installations were found.\n");
				return false;
			}
		}


		// Check for updates
		public void CheckForUpdates(string URL, string ver)
		{
			try
			{
				ConsoleData += ("[INFO] >> Checking for updates...\n");
				using (WebClient client = new WebClient())
				{
					ServicePointManager.Expect100Continue = true;
					ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
					client.Headers.Add("user-agent", "Anything");
					VersionWEB = client.DownloadString(URL).Replace("\n", "");
				}
				ConsoleData += ("[INFO] >> Got Version \"" + VersionWEB + "\"\n");
				if (VersionWEB != ver)
				{
					ConsoleData += ("[INFO] >> New version " + VersionWEB + " is availiable!\n");
					DialogResult downloadNew = MessageBox.Show("A new version is availiable! Would you like to download it now?", "New Version Availiable", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(downloadNew == DialogResult.Yes)
                    {
						Process.Start(ReleasesURL);
                    }
				}
				else
				{
					ConsoleData += ("[INFO] >> No new versions found.\n");
				}
				ConsoleData += ("");
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERORR] >> Failed to get version information: " + EX.Message + "\n");
			}
		}


		// Make the required directories (If they don't exist)
		public void MakeDirs()
		{
			ConsoleData += ("[INFO] >> Making Directories...\n");
			try
			{
				if (!Directory.Exists("./mcdata"))
				{
					ConsoleData += ("[INFO] >> Creating Directory \"./mcdata\"...\n");
					Directory.CreateDirectory("./mcdata");
				}
				else
				{
					ConsoleData += ("\n[WARNING] >> Directory \"./mcdata\" Already exists!\n");
				}

				if (!Directory.Exists("./bin"))
				{
					ConsoleData += ("[INFO] >> Creating Directory \"./bin\"...\n");
					Directory.CreateDirectory("./bin");
				}
				else
				{
					ConsoleData += ("[WARNING] >> Directory \"./bin\" Already exists!\n");
				}

				if (!Directory.Exists("./mcdata/.minecraft"))
				{
					ConsoleData += ("[INFO] >> Creating Directory \"./mcdata/.minecraft\"...\n");
					Directory.CreateDirectory("./mcdata/.minecraft");
				}
				else
				{
					ConsoleData += ("[WARNING] >> Directory \"./mcdata/.minecraft\" Already exists!\n");
				}

				if (!Directory.Exists("./bin/JRE"))
				{
					ConsoleData += ("[INFO] >> Creating Directory \"./bin/JRE\"...\n");
					Directory.CreateDirectory("./bin/JRE");
				}
				else
				{
					ConsoleData += ("[WARNING] >> Directory \"./bin/JRE\" Already exists!\n");
				}
				ConsoleData += ("[DONE]\n\n");
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
			}
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ConsoleData != PrevConsoleData)
            {
				PrevConsoleData = ConsoleData;
				richTextBox1.Text = ConsoleData;
			}
            if (GameRunning)
            {
				button1.Text = "Game is running";
				button2.Text = "Game is running";
				button3.Text = "Game is running";
				button1.Enabled = false;
				button2.Enabled = false;
				button3.Enabled = false;
			}
            else
            {
				button1.Text = "Start with official launcher";
				button2.Text = "Start with JRE";
				button3.Text = "Install OptiFine";
				button1.Enabled = true;
				button2.Enabled = true;
				button3.Enabled = true;
			}
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
			// set the current caret position to the end
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			// scroll it automatically
			richTextBox1.ScrollToCaret();
		}

        private void button4_Click(object sender, EventArgs e)
        {
			richTextBox1.Clear();
			ConsoleData = "";				
		}

        private void button2_Click(object sender, EventArgs e)
        {
			if (CheckForInstallations(mc_path) == false)
			{
				MicrosoftLoginForm loginForm = new MicrosoftLoginForm();
				loginForm.LoadingText = "Loading...";
				loginForm.Icon = ActiveForm.Icon;
				loginForm.Text = "PMCL - Login";
				MSession session = loginForm.ShowLoginDialog();
				if (session != null)
				{
					GameKey = Registry.CurrentUser.OpenSubKey("UNAME", true);
					try
					{
						GameKey = Registry.CurrentUser.CreateSubKey("UNAME", true);
					}
					catch
					{

					}
					AccessToken = session.AccessToken;
					GameKey.SetValue("AccessToken", AccessToken);
					GameKey.SetValue("Username", session.Username);
					GameKey.SetValue("UUID", session.UUID);
					GameKey.Close();


					Thread thread = new Thread(() =>
					{
						MakeDirs();
						DownloadJRE();
						Download_PY_Launcher();
						Download_MC();
						RunGame();
						GameRunning = false;
					});
					thread.Start();
					GameRunning = true;
				}
                else
                {
					MessageBox.Show("Failed to login");					
				}
			}
			else
			{
				DialogResult Continue = MessageBox.Show("An installation already exists! Do you still want to continue?", "Installation Exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (Continue == DialogResult.Yes)
				{
					Thread thread = new Thread(() =>
					{
						MakeDirs();
						DownloadJRE();
						Download_PY_Launcher();
						Download_MC();
						RunGame();
						GameRunning = false;
					});
					thread.Start();
					GameRunning = true;
				}
				else
				{
					ConsoleData += ("[WARNING] >> Operation Aborted.");

				}
			}
		}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameRunning)
            {
				DialogResult exit = MessageBox.Show("There is a game instance running! Are you sure you want to exit?", "A Game is Running!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if(exit == DialogResult.Yes)
                {

                }
                else
                {
					return;
                }
            }
			Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
			try
			{
				Thread thread = new Thread(() =>
				{
					DownloadOptifine();
					ConsoleData += ("[INFO] >> Starting Optifine installer...\n");
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
							ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
						}
						ConsoleData += ("[DONE]\n\n");
						GameRunning = false;
					}
					catch
					{

					}
				});
				thread.Start();
				GameRunning = true;
			}
			catch (Exception EX)
			{
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
			}
		}

        private void button5_Click(object sender, EventArgs e)
        {
            try
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();

				saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
				saveFileDialog1.FilterIndex = 1;
				saveFileDialog1.RestoreDirectory = true;

				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					File.WriteAllText(saveFileDialog1.FileName, ConsoleData);
				}
			}
            catch(Exception EX)
            {
				ConsoleData += ("[ERROR] >> Operation Failed.\nDETAILS: " + EX.Message + "\n");
			}
		}


		private static DialogResult ShowInputDialogBox(ref string input, string prompt, string title = "Title", int width = 300, int height = 200)
		{
			//This function creates the custom input dialog box by individually creating the different window elements and adding them to the dialog box

			//Specify the size of the window using the parameters passed
			Size size = new Size(width, height);

			//Create a new form using a System.Windows Form
			Form inputBox = new Form();
			inputBox.ControlBox = false;

			inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
			inputBox.ClientSize = size;
			//Set the window title using the parameter passed
			inputBox.Text = title;

			//Create a new label to hold the prompt
			Label label = new Label();
			label.Text = prompt;
			label.Location = new Point(5, 5);
			label.Width = size.Width - 10;
			inputBox.Controls.Add(label);

			//Create a textbox to accept the user's input
			TextBox textBox = new TextBox();
			textBox.Size = new Size(size.Width - 10, 23);
			textBox.Location = new Point(5, label.Location.Y + 20);
			textBox.Text = input;
			inputBox.Controls.Add(textBox);

			//Create an OK Button 
			Button okButton = new Button();
			okButton.DialogResult = DialogResult.OK;
			okButton.Name = "okButton";
			okButton.Size = new Size(75, 23);
			okButton.Text = "&OK";
			okButton.Location = new Point(size.Width - 80 - 80, size.Height - 30);
			inputBox.Controls.Add(okButton);

			//Create a Cancel Button
			Button cancelButton = new Button();
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 23);
			cancelButton.Text = "&Cancel";
			cancelButton.Location = new Point(size.Width - 80, size.Height - 30);
			inputBox.Controls.Add(cancelButton);

			//Set the input box's buttons to the created OK and Cancel Buttons respectively so the window appropriately behaves with the button clicks
			inputBox.AcceptButton = okButton;
			inputBox.CancelButton = cancelButton;

			//Show the window dialog box 
			DialogResult result = inputBox.ShowDialog();
			input = textBox.Text;

			//After input has been submitted, return the input value
			if(result == DialogResult.OK)
            {
				return result;
			}
			return DialogResult.Retry;

		}

		private void changeUsernameToolStripMenuItem_Click(object sender, EventArgs e)
        {
			ChangeUsername();
		}

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
			MicrosoftLoginForm loginForm = new MicrosoftLoginForm();
			loginForm.LoadingText = "Loading...";
			loginForm.Icon = ActiveForm.Icon;
			loginForm.Text = "PMCL - Logout";
			loginForm.ShowLogoutDialog();
		}

        private void loginToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			MicrosoftLoginForm loginForm = new MicrosoftLoginForm();
			loginForm.LoadingText = "Loading...";
			loginForm.Icon = ActiveForm.Icon;
			loginForm.Text = "PMCL - Login";
			MSession session = loginForm.ShowLoginDialog();
			if (session != null)
			{
				GameKey = Registry.CurrentUser.OpenSubKey("UNAME", true);
				try
				{
					GameKey = Registry.CurrentUser.CreateSubKey("UNAME", true);
				}
				catch
				{

				}
				AccessToken = session.AccessToken;
				GameKey.SetValue("AccessToken", AccessToken);
				GameKey.SetValue("Username", session.Username);
				GameKey.SetValue("UUID", session.UUID);
				GameKey.Close();
			}
			
        }
    }
}
