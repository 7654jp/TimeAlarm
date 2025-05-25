using System;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

public class Program
{
	private const string VERSION = "1.7";
	private static Form SU_form;
	private static Form TN_form;
	private static async Task TN_wait_ten_sec()
	{
		if (VERSION.Contains("TEST"))
		{ Console.WriteLine("Rest test: 1 sec."); await Task.Delay(1000); }
		else
		{ await Task.Delay(10 * 1000); }
	}
	private static bool ShowTNform(uint time, char mode)
	{
		bool dismisspressed = false;
		bool innerClose = false;
		TN_form = new Form();
		TN_form.Text = "TimeAlarm - Time to have some break!";
		TN_form.BackColor = Color.FromArgb(255, 19, 70, 19);
		TN_form.FormBorderStyle = FormBorderStyle.FixedSingle;
		TN_form.WindowState = FormWindowState.Maximized;
		TN_form.ControlBox = false;
		TN_form.TopMost = true;
		TN_form.FormClosing += (object sender, FormClosingEventArgs e) => {
			if (!(innerClose || dismisspressed))
				e.Cancel = true;
		};
		//Title label
		Label TFTitle = new Label();
		TFTitle.Font = new Font("Meiryo UI", 32, FontStyle.Bold);
		TFTitle.Text = time.ToString() + " minutes have passed!";
		TFTitle.Location = new Point(40, 20);
		TFTitle.AutoSize = false;
		TFTitle.Size = TFTitle.PreferredSize;
		TFTitle.ForeColor = Color.Yellow;
		//Description label
		Label TFDesc = new Label();
		TFDesc.Font = new Font("Meiryo UI", 24, FontStyle.Bold);
		TFDesc.Text = "You should rest your eyes.\nLet's have some break!";
		TFDesc.Location = new Point(50, 200);
		TFDesc.AutoSize = false;
		TFDesc.Size = TFDesc.PreferredSize;
		TFDesc.ForeColor = Color.Yellow;
		if (mode == 'U')
		{
			Button TFClose = new Button();
			TFClose.Font = new Font("Meiryo UI", 24);
			TFClose.Text = "OK";
			TFClose.Location = new Point(1600, 900);
			TFClose.BackColor = Color.FromName("Control");
			TFClose.AutoSize = false;
			TFClose.Size = TFClose.PreferredSize;
			TFClose.Click += (sender, e) => { innerClose = true; TN_form.Close(); };
			TN_form.Controls.Add(TFTitle);
			TN_form.Controls.Add(TFDesc);
			TN_form.Controls.Add(TFClose);
			TN_form.ShowDialog();
			return false;
		}
		//Dismiss label
		Label TFDismissDesc = new Label();
		TFDismissDesc.Font = new Font("Meiryo UI", 12);
		TFDismissDesc.Text = "Enter 'Dismiss' below\nThen press Escape key";
		TFDismissDesc.ForeColor = Color.Yellow;
		TFDismissDesc.Location = new Point(50, 800);
		TFDismissDesc.AutoSize = false;
		TFDismissDesc.Size = TFDismissDesc.PreferredSize;
		//Dismiss textbox
		TextBox TFBDismiss = new TextBox();
		TFBDismiss.Font = new Font("Meiryo UI", 12);
		TFBDismiss.Location = new Point(50, 900);
		TFBDismiss.Text = "This will be enough";
		TFBDismiss.AutoSize = false;
		TFBDismiss.Size = TFBDismiss.PreferredSize;
		TFBDismiss.Text = "";
		TFBDismiss.KeyDown += (object sender, KeyEventArgs e) => {
			if (TFBDismiss.Text == "Dismiss" && e.KeyCode == Keys.Escape) { dismisspressed = true; TN_form.Close(); } };
		//Break button
		Button TFBreak = new Button();
		TFBreak.Font = new Font("Meiryo UI", 24);
		TFBreak.Text = "Have a break of\n10 minutes";
		TFBreak.Location = new Point(1350, 850);
		TFBreak.BackColor = Color.FromName("Control");
		TFBreak.AutoSize = false;
		TFBreak.Size = TFBreak.PreferredSize;
		TFBreak.Click += async (object sender, EventArgs e) => {
			TFDismissDesc.Visible = false;
			TFBDismiss.Visible = false;
			TFDesc.Text = "Eye resting: 10 minute(s) left";
			TFDesc.Size = TFDesc.PreferredSize;
			TFDesc.Refresh();
			Console.WriteLine("Started rest counting");
			TFBreak.Enabled = false;
			for (int i = 10; i > 0;)
			{
				i--;
				for (int j = 50; j >= 0; j -= 10)
				{
					await TN_wait_ten_sec();
					TFDesc.Text = "Eye resting: " + i.ToString() + " minute(s) ";
					if (j != 0)
						TFDesc.Text += j.ToString() + " second(s) ";
					TFDesc.Text += "left";
					TFDesc.Size = TFDesc.PreferredSize;
				}
			}
			Console.WriteLine("Ended rest counting");
			TFBreak.Visible = false;
			Button TFQuit = new Button();
			TFQuit.Font = new Font("Meiryo UI", 24);
			TFQuit.Text = "Have a break of\n10 minutes";
			TFQuit.Location = new Point(1350, 850);
			TFQuit.BackColor = Color.FromName("Control");
			TFQuit.AutoSize = false;
			TFQuit.Size = TFBreak.Size;
			TFQuit.Text = "End resting";
			TFQuit.Click += (object csender, EventArgs ce) => { innerClose = true; TN_form.Close(); };
			TN_form.Controls.Add(TFQuit);
			TFQuit.Visible = true;
			Console.WriteLine("End resting");
		};
		//Add control & Run
		TN_form.Controls.Add(TFTitle);
		TN_form.Controls.Add(TFDesc);
		TN_form.Controls.Add(TFDismissDesc);
		TN_form.Controls.Add(TFBDismiss);
		TN_form.Controls.Add(TFBreak);
		TN_form.ShowDialog();
		return dismisspressed;
	}
	private static void NotifyAsync(uint time)
	{
		Thread MsgThread = new Thread(new ThreadStart(() => {
			MessageBox.Show(time.ToString() + " minutes have passed!", "TimeAlarm - Time notify", MessageBoxButtons.OK,
			MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
		}));
		MsgThread.Start();
	}
	public static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("Error: Number of arguments was not 1.");
			Console.WriteLine("Usage: TimeAlarm.exe [-OnStartUp/-OnUserCall]");
			return;
		}
		Console.WriteLine(@"  _______                ___    __                   ");
		Console.WriteLine(@" /_  __(_)___ ___  ___  /   |  / /___ __________ ___ ");
		Console.WriteLine(@"  / / / / __ `__ \/ _ \/ /| | / / __ `/ ___/ __ `__ \");
		Console.WriteLine(@" / / / / / / / / /  __/ ___ |/ / /_/ / /  / / / / / /");
		Console.WriteLine(@"/_/ /_/_/ /_/ /_/\___/_/  |_/_/\__,_/_/  /_/ /_/ /_/ ");
		Console.WriteLine(@"=====================================================");
		Console.WriteLine("Made by @7654jp, Version " + VERSION);
		bool innerClose = false;
		//Argument
		if (args[0] == "-OnStartUp")
		{
			//Form init
			SU_form = new Form();
			#region StartUp Form
			SU_form.Text = "TimeAlarm Start-Up Window";
			SU_form.BackColor = Color.FromArgb(255, 19, 70, 19);
			SU_form.FormBorderStyle = FormBorderStyle.FixedSingle;
			SU_form.WindowState = FormWindowState.Maximized;
			SU_form.FormClosing += (object sender, FormClosingEventArgs e) => {
				if (!innerClose)
				{ e.Cancel = true; };
			};
			SU_form.ControlBox = false;
			SU_form.TopMost = true;
			//Title label
			Label SFTitle = new Label();
			SFTitle.Font = new Font("Meiryo UI", 32, FontStyle.Bold);
			SFTitle.Text = "TimeAlarm - Elapsed time counter";
			SFTitle.Location = new Point(40, 20);
			SFTitle.AutoSize = false;
			SFTitle.Size = SFTitle.PreferredSize;
			SFTitle.ForeColor = Color.Yellow;
			//Description label
			Label SFDesc = new Label();
			SFDesc.Font = new Font("Meiryo UI", 24, FontStyle.Bold);
			SFDesc.Text = "This program will count time to\nprevents you from using the computer continuously\nfor more than 50 minutes.\n\nMade by @7654jp, Version " + VERSION;
			SFDesc.Location = new Point(50, 200);
			SFDesc.AutoSize = false;
			SFDesc.Size = SFDesc.PreferredSize;
			SFDesc.ForeColor = Color.Yellow;
			//Continue button
			Button SFCont = new Button();
			SFCont.Font = new Font("Meiryo UI", 24);
			SFCont.Text = "Start using this computer";
			SFCont.BackColor = Color.FromName("Control");
			SFCont.Location = new Point(1300, 900);
			SFCont.AutoSize = false;
			SFCont.Size = SFCont.PreferredSize;
			SFCont.Click += (object sender, EventArgs e) => { innerClose = true; SU_form.Close(); };
			//Add control & Run
			SU_form.Controls.Add(SFTitle);
			SU_form.Controls.Add(SFDesc);
			SU_form.Controls.Add(SFCont);
			#endregion
			SU_form.ShowDialog();
			//Count
			uint min = 0;
			bool Dismissed = false;
			if (VERSION.Contains("TEST"))
			{ Console.WriteLine("Count test: 1 sec."); }
			while (true)
			{
				if (VERSION.Contains("TEST"))
				{ Thread.Sleep(1000); }
				else
				{ Thread.Sleep(5 * 60 * 1000); }
				min += 5;
				Console.WriteLine(min.ToString() + " minutes");
				if (min % 60 == 50 || Dismissed)
				{
					Console.WriteLine("Time notified");
					Dismissed = ShowTNform(min, 'S');
					continue;
				}
				if (min % 10 == 5)
				{ continue; }
				NotifyAsync(min);
			}
		}
		else if (args[0] == "-OnUserCall")
		{
			Console.Write("Enter 'no' to disable 10-minutes messagebox>");
			bool tenmin_msg = (Console.ReadLine().ToLower() != "no");
			uint minafter = 0;
			//Get valid number
			while (true)
			{
				try
				{
					Console.Write("\nNotify after (minutes):");
					minafter = uint.Parse(Console.ReadLine());
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
					continue;
				}
				break;
			}
			//
			uint count = 0;
			while (true)
			{
				if (minafter == count)
				{ break; }
				Thread.Sleep(60 * 1000);
				count++;
				if (count % 10 == 0 && tenmin_msg)
				{ NotifyAsync(count); }
			}
			ShowTNform(count, 'U');
		}
		else
		{
			Console.WriteLine("Error: Unknown argument: " + args[0]);
		}
		return;
	}
}
