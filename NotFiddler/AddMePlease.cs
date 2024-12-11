using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace NotFiddler;

public class AddMePlease : Form
{
	public Form1 MainForm;

	private string CloudSteam;

	private string DesiredNickname;

	private bool AutoRemove;

	private IContainer components;

	private Label label1;

	private TextBox textBox1;

	private Label label2;

	private TextBox textBox2;

	private CheckBox checkBox1;

	private Button button1;

	public AddMePlease(Form1 parent)
	{
		InitializeComponent();
		MainForm = parent;
		CloudSteam = "";
		DesiredNickname = "";
		AutoRemove = false;
	}

	private void SeMyButton(bool enabled)
	{
		if (base.InvokeRequired)
		{
			button1.Invoke(new Action<bool>(SeMyButton), enabled);
		}
		else
		{
			button1.Enabled = enabled;
		}
	}

	private void SetCloudSteam(string newVal)
	{
		if (base.InvokeRequired)
		{
			button1.Invoke(new Action<string>(SetCloudSteam), newVal);
		}
		else
		{
			textBox1.Text = newVal;
		}
	}

	private string TryParseFriends(string steam64)
	{
		string text = MainForm.DBD_GetMyFriends();
		try
		{
			foreach (JObject item in JArray.Parse(text))
			{
				if (item["platformIds"]["steam"] != null && (string?)item["platformIds"]["steam"] == steam64)
				{
					string text2 = (string?)item["friendId"];
					MainForm.OutputLog("[TryParseFriends] CloudID: " + text2 + ": Steam64: " + steam64);
					return text2;
				}
			}
		}
		catch (Exception ex)
		{
			MainForm.OutputLog("[Sender] something went wrong in TryParseFriends (" + ex.Message + "): " + text);
		}
		return "";
	}

	private void MyThread()
	{
		string text = "";
		bool flag = MainForm.AppliedNickname == DesiredNickname;
		if (!flag)
		{
			text = MainForm.DBD_ChangeNickname(DesiredNickname);
			flag = text.Contains("\"userId\":");
		}
		if (flag)
		{
			MainForm.AppliedNickname = DesiredNickname;
			do
			{
				string text2 = "kraken";
				if (!CloudSteam.Contains("-"))
				{
					string text3 = TryParseFriends(CloudSteam);
					if (text3.Length > 0)
					{
						CloudSteam = text3;
						SetCloudSteam(CloudSteam);
					}
					else
					{
						text2 = "steam";
					}
				}
				MainForm.OutputLog($"[Sender] {CloudSteam} ({text2}) DN: {DesiredNickname} AR: {AutoRemove}");
				string text4 = MainForm.DBD_AddRemoveFriend(CloudSteam, text2, add: true);
				if (text4.Contains("\"friends\":"))
				{
					if (text2 == "steam")
					{
						string text5 = TryParseFriends(CloudSteam);
						if (text5.Length > 0)
						{
							CloudSteam = text5;
							SetCloudSteam(CloudSteam);
							continue;
						}
						MessageBox.Show("[TryParseFriends] Friend not found!\t", "Fuck...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						break;
					}
					if (AutoRemove)
					{
						string text6 = MainForm.DBD_AddRemoveFriend(CloudSteam, text2, add: false);
						if (!text6.Contains("\"friends\":"))
						{
							MainForm.OutputLog(text6);
							MessageBox.Show("[Auto Remove] Friend is not removed!\t", "Damn...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
					break;
				}
				MainForm.OutputLog(text4);
				MessageBox.Show("Can't add friend! Try remove it from friendlist\t", "Oh shit...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				string text7 = MainForm.DBD_AddRemoveFriend(CloudSteam, text2, add: false);
				MainForm.OutputLog(string.Format("[Sender] ForceRemoved: {0}", text7.Contains("\"friends\":")));
				break;
			}
			while (AutoRemove);
		}
		else
		{
			MainForm.OutputLog(text);
			MessageBox.Show("Nickname is invalid\t", "Oh no...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		SeMyButton(enabled: true);
	}

	private void button1_Click(object sender, EventArgs e)
	{
		CloudSteam = textBox1.Text;
		DesiredNickname = textBox2.Text;
		AutoRemove = checkBox1.Checked;
		if (CloudSteam.Length == 0 || DesiredNickname.Length == 0)
		{
			MessageBox.Show("Bad Inputs\t", "Dumbass", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		SeMyButton(enabled: false);
		new Thread(MyThread).Start();
	}

	private void AddMePlease_Shown(object sender, EventArgs e)
	{
		textBox1.Text = MainForm.CloudSteamCached;
		textBox2.Text = MainForm.DesiredNicknameCached;
	}

	private void AddMePlease_FormClosing(object sender, FormClosingEventArgs e)
	{
		MainForm.CloudSteamCached = textBox1.Text;
		MainForm.DesiredNicknameCached = textBox2.Text;
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
		this.label1 = new System.Windows.Forms.Label();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(126, 34);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(154, 18);
		this.label1.TabIndex = 0;
		this.label1.Text = "CloudID / Steam64:";
		this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox1.Location = new System.Drawing.Point(12, 75);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(389, 21);
		this.textBox1.TabIndex = 1;
		this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(137, 117);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(134, 18);
		this.label2.TabIndex = 2;
		this.label2.Text = "Desired Nickname:";
		this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox2.Location = new System.Drawing.Point(72, 148);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(258, 21);
		this.textBox2.TabIndex = 3;
		this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.checkBox1.AutoSize = true;
		this.checkBox1.Checked = true;
		this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox1.Location = new System.Drawing.Point(157, 184);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(91, 17);
		this.checkBox1.TabIndex = 4;
		this.checkBox1.Text = "Auto Remove";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button1.Location = new System.Drawing.Point(122, 220);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(164, 34);
		this.button1.TabIndex = 5;
		this.button1.Text = "SEND REQUEST";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(413, 291);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.textBox2);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddMePlease";
		this.Text = "Sender";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddMePlease_FormClosing);
		base.Shown += new System.EventHandler(AddMePlease_Shown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
