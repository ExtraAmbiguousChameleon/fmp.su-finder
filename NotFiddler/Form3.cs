using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NotFiddler;

public class Form3 : Form
{
	public Form1 MainForm;

	private int DelayValue;

	private bool WaitOthers;

	private bool WaitKillerOnly;

	private string WaitExcludeList;

	private IContainer components;

	private TextBox textBox1;

	private Label label1;

	private Button button1;

	private Label label2;

	private CheckBox checkBox1;

	private Label label3;

	private RichTextBox richTextBox1;

	private GroupBox groupBox1;

	private CheckBox checkBox2;

	private Label label4;

	public Form3(Form1 parent)
	{
		InitializeComponent();
		MainForm = parent;
		DelayValue = 0;
		WaitOthers = false;
		WaitExcludeList = "";
		button1.Enabled = false;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		int result = 0;
		if (int.TryParse(textBox1.Text, out result) && result > 0 && result < 999)
		{
			DelayValue = result;
			WaitOthers = checkBox1.Checked;
			WaitKillerOnly = checkBox2.Checked;
			WaitExcludeList = richTextBox1.Text;
			MainForm.SkipSameDelay = DelayValue;
			MainForm.SkipSameWaitOthers = WaitOthers;
			MainForm.SkipSameWaitKillerOnly = WaitKillerOnly;
			MainForm.SkipSameWaitExcludeList = WaitExcludeList;
			button1.Enabled = false;
			Close();
		}
		else
		{
			MessageBox.Show("Bad Inputs\t", "Dumbass", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void Form3_Shown(object sender, EventArgs e)
	{
		DelayValue = MainForm.SkipSameDelay;
		WaitOthers = MainForm.SkipSameWaitOthers;
		WaitKillerOnly = MainForm.SkipSameWaitKillerOnly;
		WaitExcludeList = MainForm.SkipSameWaitExcludeList;
		textBox1.Text = DelayValue.ToString();
		checkBox1.Checked = WaitOthers;
		checkBox2.Checked = WaitKillerOnly;
		richTextBox1.Text = WaitExcludeList;
		button1.Enabled = true;
	}

	private void checkBox1_Click(object sender, EventArgs e)
	{
		if (checkBox2.Checked)
		{
			checkBox2.Checked = false;
		}
	}

	private void checkBox2_Click(object sender, EventArgs e)
	{
		if (checkBox1.Checked)
		{
			checkBox1.Checked = false;
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
		this.button1 = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.label3 = new System.Windows.Forms.Label();
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.label4 = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.textBox1.Location = new System.Drawing.Point(173, 19);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(77, 24);
		this.textBox1.TabIndex = 0;
		this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(78, 22);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(89, 18);
		this.label1.TabIndex = 1;
		this.label1.Text = "Delay Value:";
		this.button1.Location = new System.Drawing.Point(115, 202);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(99, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "Apply Settings";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(151, 53);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(23, 13);
		this.label2.TabIndex = 3;
		this.label2.Text = "OR";
		this.checkBox1.AutoSize = true;
		this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.checkBox1.Location = new System.Drawing.Point(19, 76);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(136, 21);
		this.checkBox1.TabIndex = 5;
		this.checkBox1.Text = "Wait for full lobby";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.Click += new System.EventHandler(checkBox1_Click);
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label3.Location = new System.Drawing.Point(253, 25);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(12, 13);
		this.label3.TabIndex = 6;
		this.label3.Text = "s";
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Location = new System.Drawing.Point(6, 19);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.Size = new System.Drawing.Size(260, 60);
		this.richTextBox1.TabIndex = 7;
		this.richTextBox1.Text = "";
		this.groupBox1.Controls.Add(this.richTextBox1);
		this.groupBox1.Location = new System.Drawing.Point(31, 104);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(272, 85);
		this.groupBox1.TabIndex = 8;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Wait Exclude List (cloudid):";
		this.checkBox2.AutoSize = true;
		this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.checkBox2.Location = new System.Drawing.Point(177, 76);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(144, 21);
		this.checkBox2.TabIndex = 9;
		this.checkBox2.Text = "Wait for Killer Only";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.checkBox2.Click += new System.EventHandler(checkBox2_Click);
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label4.Location = new System.Drawing.Point(157, 81);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(12, 13);
		this.label4.TabIndex = 10;
		this.label4.Text = "/";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(333, 244);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.checkBox2);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Form3";
		this.Text = "Skip Same Lobbies Settings";
		base.Shown += new System.EventHandler(Form3_Shown);
		this.groupBox1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
