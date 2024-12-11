using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace NotFiddler;

public class Form2 : Form
{
	private Thread MyThreadHandle;

	private bool CloseRequested;

	private List<string> Players;

	private List<string> Steam64;

	public bool NeedUpdate;

	public Form1 MainForm;

	private IContainer components;

	private TextBox textBox1;

	private Label label1;

	private Label label2;

	private TextBox textBox2;

	private TextBox textBox3;

	private TextBox textBox4;

	private TextBox textBox5;

	private Button button1;

	private Button button2;

	private Button button3;

	private Button button4;

	private Button button5;

	private Button button6;

	public Form2(Form1 parent)
	{
		InitializeComponent();
		textBox1.ReadOnly = true;
		textBox1.BackColor = SystemColors.Window;
		button1.Enabled = false;
		textBox2.ReadOnly = true;
		textBox2.BackColor = SystemColors.Window;
		button2.Enabled = false;
		textBox3.ReadOnly = true;
		textBox3.BackColor = SystemColors.Window;
		button3.Enabled = false;
		textBox4.ReadOnly = true;
		textBox4.BackColor = SystemColors.Window;
		button4.Enabled = false;
		textBox5.ReadOnly = true;
		textBox5.BackColor = SystemColors.Window;
		button5.Enabled = false;
		MyThreadHandle = new Thread(MyThread);
		MyThreadHandle.Start();
		CloseRequested = false;
		Players = new List<string>();
		Steam64 = new List<string>();
		NeedUpdate = false;
		MainForm = parent;
	}

	private void RedrawRequest()
	{
		for (int i = 0; i < Players.Count; i++)
		{
			switch (i)
			{
			case 0:
				textBox1.Text = Players[i];
				button1.Enabled = Steam64[i].Length > 0;
				break;
			case 1:
				textBox2.Text = Players[i];
				button2.Enabled = Steam64[i].Length > 0;
				break;
			case 2:
				textBox3.Text = Players[i];
				button3.Enabled = Steam64[i].Length > 0;
				break;
			case 3:
				textBox4.Text = Players[i];
				button4.Enabled = Steam64[i].Length > 0;
				break;
			case 4:
				textBox5.Text = Players[i];
				button5.Enabled = Steam64[i].Length > 0;
				break;
			}
		}
	}

	private void SetCloseButton(bool enabled)
	{
		if (base.InvokeRequired)
		{
			button6.Invoke(new Action<bool>(SetCloseButton), enabled);
		}
		else
		{
			button6.Enabled = enabled;
		}
	}

	private void MyThread()
	{
		while (!CloseRequested)
		{
			if (!NeedUpdate)
			{
				continue;
			}
			Players.Clear();
			Steam64.Clear();
			bool flag = false;
			foreach (string lastPlayerId in MainForm.LastPlayerIds)
			{
				if (CloseRequested)
				{
					flag = true;
					break;
				}
				try
				{
					JObject jObject = JObject.Parse(MainForm.DBD_GetNicknameByCloudID(lastPlayerId));
					string text = (string?)jObject["playerName"];
					string text2 = "";
					if (jObject["providerPlayerNames"] != null && jObject["providerPlayerNames"]["steam"] != null)
					{
						string text3 = MainForm.DBD_GetSteamByCloudID(lastPlayerId);
						if (text3.Contains("\"providerId\":"))
						{
							text2 = (string?)JObject.Parse(text3)["providerId"];
						}
					}
					Players.Add(lastPlayerId + " | " + text + " | " + text2);
					Steam64.Add(text2);
					Invoke(new Action(RedrawRequest));
				}
				catch (Exception ex)
				{
					MainForm.OutputLog("something went wrong in my thread (" + ex.Message + ")");
				}
			}
			if (!flag)
			{
				MainForm.PlayersCached = Players;
				MainForm.Steam64Cached = Steam64;
			}
			NeedUpdate = false;
			SetCloseButton(enabled: true);
		}
	}

	private void Form2_Shown(object sender, EventArgs e)
	{
		if (NeedUpdate)
		{
			button6.Enabled = false;
		}
		else if (MainForm.PlayersCached.Count == 0 || MainForm.PlayersCached.Count != MainForm.Steam64Cached.Count)
		{
			NeedUpdate = true;
			button6.Enabled = false;
		}
		else
		{
			Players = MainForm.PlayersCached;
			Steam64 = MainForm.Steam64Cached;
			Invoke(new Action(RedrawRequest));
		}
	}

	private void Form2_FormClosing(object sender, FormClosingEventArgs e)
	{
		CloseRequested = true;
	}

	private void TryOpenSteamProfile(int index)
	{
		if (index < Steam64.Count)
		{
			Process.Start("https://steamcommunity.com/profiles/" + Steam64[index]);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		TryOpenSteamProfile(0);
	}

	private void button2_Click(object sender, EventArgs e)
	{
		TryOpenSteamProfile(1);
	}

	private void button3_Click(object sender, EventArgs e)
	{
		TryOpenSteamProfile(2);
	}

	private void button4_Click(object sender, EventArgs e)
	{
		TryOpenSteamProfile(3);
	}

	private void button5_Click(object sender, EventArgs e)
	{
		TryOpenSteamProfile(4);
	}

	private void button6_Click(object sender, EventArgs e)
	{
		button6.Enabled = false;
		Close();
	}

	private void textBox1_MouseClick(object sender, MouseEventArgs e)
	{
		textBox1.SelectAll();
	}

	private void textBox2_Click(object sender, EventArgs e)
	{
		textBox2.SelectAll();
	}

	private void textBox3_Click(object sender, EventArgs e)
	{
		textBox3.SelectAll();
	}

	private void textBox4_Click(object sender, EventArgs e)
	{
		textBox4.SelectAll();
	}

	private void textBox5_Click(object sender, EventArgs e)
	{
		textBox5.SelectAll();
	}

	private void label2_DoubleClick(object sender, EventArgs e)
	{
		if (!NeedUpdate)
		{
			string text = "";
			for (int i = 1; i < Players.Count; i++)
			{
				text = text + Players[i] + "\n";
			}
			if (text.Length > 0)
			{
				Clipboard.SetText(text);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.textBox3 = new System.Windows.Forms.TextBox();
		this.textBox4 = new System.Windows.Forms.TextBox();
		this.textBox5 = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		this.button3 = new System.Windows.Forms.Button();
		this.button4 = new System.Windows.Forms.Button();
		this.button5 = new System.Windows.Forms.Button();
		this.button6 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox1.Location = new System.Drawing.Point(7, 47);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(360, 23);
		this.textBox1.TabIndex = 0;
		this.textBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(textBox1_MouseClick);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(177, 12);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(69, 18);
		this.label1.TabIndex = 1;
		this.label1.Text = "KILLER:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(168, 90);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(84, 18);
		this.label2.TabIndex = 2;
		this.label2.Text = "Survivors:";
		this.label2.DoubleClick += new System.EventHandler(label2_DoubleClick);
		this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox2.Location = new System.Drawing.Point(7, 124);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(360, 23);
		this.textBox2.TabIndex = 3;
		this.textBox2.Click += new System.EventHandler(textBox2_Click);
		this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox3.Location = new System.Drawing.Point(7, 158);
		this.textBox3.Name = "textBox3";
		this.textBox3.Size = new System.Drawing.Size(360, 23);
		this.textBox3.TabIndex = 4;
		this.textBox3.Click += new System.EventHandler(textBox3_Click);
		this.textBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox4.Location = new System.Drawing.Point(7, 192);
		this.textBox4.Name = "textBox4";
		this.textBox4.Size = new System.Drawing.Size(360, 23);
		this.textBox4.TabIndex = 5;
		this.textBox4.Click += new System.EventHandler(textBox4_Click);
		this.textBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox5.Location = new System.Drawing.Point(7, 226);
		this.textBox5.Name = "textBox5";
		this.textBox5.Size = new System.Drawing.Size(360, 23);
		this.textBox5.TabIndex = 6;
		this.textBox5.Click += new System.EventHandler(textBox5_Click);
		this.button1.Enabled = false;
		this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button1.Location = new System.Drawing.Point(369, 47);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(42, 23);
		this.button1.TabIndex = 7;
		this.button1.Text = "STEAM";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.Enabled = false;
		this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button2.Location = new System.Drawing.Point(369, 124);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(42, 23);
		this.button2.TabIndex = 8;
		this.button2.Text = "STEAM";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.button3.Enabled = false;
		this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button3.Location = new System.Drawing.Point(369, 158);
		this.button3.Name = "button3";
		this.button3.Size = new System.Drawing.Size(42, 23);
		this.button3.TabIndex = 9;
		this.button3.Text = "STEAM";
		this.button3.UseVisualStyleBackColor = true;
		this.button3.Click += new System.EventHandler(button3_Click);
		this.button4.Enabled = false;
		this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button4.Location = new System.Drawing.Point(369, 192);
		this.button4.Name = "button4";
		this.button4.Size = new System.Drawing.Size(42, 23);
		this.button4.TabIndex = 10;
		this.button4.Text = "STEAM";
		this.button4.UseVisualStyleBackColor = true;
		this.button4.Click += new System.EventHandler(button4_Click);
		this.button5.Enabled = false;
		this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button5.Location = new System.Drawing.Point(369, 226);
		this.button5.Name = "button5";
		this.button5.Size = new System.Drawing.Size(42, 23);
		this.button5.TabIndex = 11;
		this.button5.Text = "STEAM";
		this.button5.UseVisualStyleBackColor = true;
		this.button5.Click += new System.EventHandler(button5_Click);
		this.button6.Location = new System.Drawing.Point(171, 266);
		this.button6.Name = "button6";
		this.button6.Size = new System.Drawing.Size(75, 23);
		this.button6.TabIndex = 12;
		this.button6.Text = "Close";
		this.button6.UseVisualStyleBackColor = true;
		this.button6.Click += new System.EventHandler(button6_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(417, 305);
		base.Controls.Add(this.button6);
		base.Controls.Add(this.button5);
		base.Controls.Add(this.button4);
		base.Controls.Add(this.button3);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBox5);
		base.Controls.Add(this.textBox4);
		base.Controls.Add(this.textBox3);
		base.Controls.Add(this.textBox2);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Form2";
		this.Text = "Match Player List";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form2_FormClosing);
		base.Shown += new System.EventHandler(Form2_Shown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
