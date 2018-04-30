namespace YueWoolworthVoucherDetector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartButton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CheckDupeRangeButton = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.findLongNum = new System.Windows.Forms.Button();
            this.buttonUpdateInsuranceUpdate = new System.Windows.Forms.Button();
            this.buttonUtil = new System.Windows.Forms.Button();
            this.SetToAllButton = new System.Windows.Forms.Button();
            this.textBoxUdid = new System.Windows.Forms.TextBox();
            this.textBoxDigest = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.StopButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxFolders = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ExtractButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSetToning0 = new System.Windows.Forms.Button();
            this.DeleteDupeButton = new System.Windows.Forms.Button();
            this.CheckDupeButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.RunInListButton = new System.Windows.Forms.Button();
            this.StartAllButton = new System.Windows.Forms.Button();
            this.StopAllButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(12, 272);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(57, 56);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(590, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(549, 448);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CheckDupeRangeButton);
            this.groupBox2.Controls.Add(this.numericUpDown2);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(432, 86);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Range";
            // 
            // CheckDupeRangeButton
            // 
            this.CheckDupeRangeButton.Location = new System.Drawing.Point(313, 17);
            this.CheckDupeRangeButton.Name = "CheckDupeRangeButton";
            this.CheckDupeRangeButton.Size = new System.Drawing.Size(113, 61);
            this.CheckDupeRangeButton.TabIndex = 4;
            this.CheckDupeRangeButton.Text = "Check Dupe Range";
            this.CheckDupeRangeButton.UseVisualStyleBackColor = true;
            this.CheckDupeRangeButton.Click += new System.EventHandler(this.CheckDupeRangeButton_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(49, 58);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            -1981284352,
            -1966660860,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(258, 20);
            this.numericUpDown2.TabIndex = 3;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(49, 20);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            -1981284352,
            -1966660860,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(258, 20);
            this.numericUpDown1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "From";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.findLongNum);
            this.groupBox1.Controls.Add(this.buttonUpdateInsuranceUpdate);
            this.groupBox1.Controls.Add(this.buttonUtil);
            this.groupBox1.Controls.Add(this.SetToAllButton);
            this.groupBox1.Controls.Add(this.textBoxUdid);
            this.groupBox1.Controls.Add(this.textBoxDigest);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxKey);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 157);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Keys";
            // 
            // findLongNum
            // 
            this.findLongNum.Location = new System.Drawing.Point(198, 107);
            this.findLongNum.Name = "findLongNum";
            this.findLongNum.Size = new System.Drawing.Size(87, 44);
            this.findLongNum.TabIndex = 7;
            this.findLongNum.Text = "find long num";
            this.findLongNum.UseVisualStyleBackColor = true;
            this.findLongNum.Click += new System.EventHandler(this.findLongNum_Click);
            // 
            // buttonUpdateInsuranceUpdate
            // 
            this.buttonUpdateInsuranceUpdate.Location = new System.Drawing.Point(110, 107);
            this.buttonUpdateInsuranceUpdate.Name = "buttonUpdateInsuranceUpdate";
            this.buttonUpdateInsuranceUpdate.Size = new System.Drawing.Size(82, 44);
            this.buttonUpdateInsuranceUpdate.TabIndex = 6;
            this.buttonUpdateInsuranceUpdate.Text = "Util: update Insurance";
            this.buttonUpdateInsuranceUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdateInsuranceUpdate.Click += new System.EventHandler(this.buttonUpdateInsuranceUpdate_Click_1);
            // 
            // buttonUtil
            // 
            this.buttonUtil.Location = new System.Drawing.Point(9, 107);
            this.buttonUtil.Name = "buttonUtil";
            this.buttonUtil.Size = new System.Drawing.Size(95, 44);
            this.buttonUtil.TabIndex = 6;
            this.buttonUtil.Text = "Util: update year end date";
            this.buttonUtil.UseVisualStyleBackColor = true;
            this.buttonUtil.Click += new System.EventHandler(this.buttonUtil_Click_1);
            // 
            // SetToAllButton
            // 
            this.SetToAllButton.Location = new System.Drawing.Point(318, 107);
            this.SetToAllButton.Name = "SetToAllButton";
            this.SetToAllButton.Size = new System.Drawing.Size(108, 44);
            this.SetToAllButton.TabIndex = 5;
            this.SetToAllButton.Text = "Set To All";
            this.SetToAllButton.UseVisualStyleBackColor = true;
            this.SetToAllButton.Click += new System.EventHandler(this.SetToAllButton_Click);
            // 
            // textBoxUdid
            // 
            this.textBoxUdid.Location = new System.Drawing.Point(32, 81);
            this.textBoxUdid.Name = "textBoxUdid";
            this.textBoxUdid.Size = new System.Drawing.Size(394, 20);
            this.textBoxUdid.TabIndex = 4;
            // 
            // textBoxDigest
            // 
            this.textBoxDigest.Location = new System.Drawing.Point(32, 51);
            this.textBoxDigest.Name = "textBoxDigest";
            this.textBoxDigest.Size = new System.Drawing.Size(394, 20);
            this.textBoxDigest.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Udid";
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(32, 20);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(394, 20);
            this.textBoxKey.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Digest";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Key";
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(79, 272);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(60, 56);
            this.StopButton.TabIndex = 5;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Find $20 voucher: 0";
            // 
            // textBoxFolders
            // 
            this.textBoxFolders.Location = new System.Drawing.Point(67, 52);
            this.textBoxFolders.Name = "textBoxFolders";
            this.textBoxFolders.Size = new System.Drawing.Size(359, 20);
            this.textBoxFolders.TabIndex = 7;
            this.textBoxFolders.Text = "yue,yue2,yue3,yue4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Folders:";
            // 
            // ExtractButton
            // 
            this.ExtractButton.Location = new System.Drawing.Point(9, 80);
            this.ExtractButton.Name = "ExtractButton";
            this.ExtractButton.Size = new System.Drawing.Size(118, 40);
            this.ExtractButton.TabIndex = 9;
            this.ExtractButton.Text = "Extract";
            this.ExtractButton.UseVisualStyleBackColor = true;
            this.ExtractButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSetToning0);
            this.groupBox3.Controls.Add(this.DeleteDupeButton);
            this.groupBox3.Controls.Add(this.CheckDupeButton);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.ExtractButton);
            this.groupBox3.Controls.Add(this.textBoxFolders);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(12, 334);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(432, 126);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Extract";
            // 
            // buttonSetToning0
            // 
            this.buttonSetToning0.Location = new System.Drawing.Point(323, 18);
            this.buttonSetToning0.Name = "buttonSetToning0";
            this.buttonSetToning0.Size = new System.Drawing.Size(103, 23);
            this.buttonSetToning0.TabIndex = 12;
            this.buttonSetToning0.Text = "Reset Tonight = 0";
            this.buttonSetToning0.UseVisualStyleBackColor = true;
            this.buttonSetToning0.Click += new System.EventHandler(this.buttonSetToning0_Click);
            // 
            // DeleteDupeButton
            // 
            this.DeleteDupeButton.Location = new System.Drawing.Point(274, 80);
            this.DeleteDupeButton.Name = "DeleteDupeButton";
            this.DeleteDupeButton.Size = new System.Drawing.Size(117, 40);
            this.DeleteDupeButton.TabIndex = 11;
            this.DeleteDupeButton.Text = "Delete Dupe";
            this.DeleteDupeButton.UseVisualStyleBackColor = true;
            this.DeleteDupeButton.Click += new System.EventHandler(this.button5_Click);
            // 
            // CheckDupeButton
            // 
            this.CheckDupeButton.Location = new System.Drawing.Point(136, 80);
            this.CheckDupeButton.Name = "CheckDupeButton";
            this.CheckDupeButton.Size = new System.Drawing.Size(120, 40);
            this.CheckDupeButton.TabIndex = 10;
            this.CheckDupeButton.Text = "Check Dupe";
            this.CheckDupeButton.UseVisualStyleBackColor = true;
            this.CheckDupeButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(543, 469);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Mushroom @ 12/07/2017";
            // 
            // RunInListButton
            // 
            this.RunInListButton.Location = new System.Drawing.Point(335, 272);
            this.RunInListButton.Name = "RunInListButton";
            this.RunInListButton.Size = new System.Drawing.Size(85, 56);
            this.RunInListButton.TabIndex = 12;
            this.RunInListButton.Text = "Run in List";
            this.RunInListButton.UseVisualStyleBackColor = true;
            this.RunInListButton.Click += new System.EventHandler(this.button6_Click);
            // 
            // StartAllButton
            // 
            this.StartAllButton.Location = new System.Drawing.Point(148, 272);
            this.StartAllButton.Name = "StartAllButton";
            this.StartAllButton.Size = new System.Drawing.Size(84, 56);
            this.StartAllButton.TabIndex = 13;
            this.StartAllButton.Text = "Start All";
            this.StartAllButton.UseVisualStyleBackColor = true;
            this.StartAllButton.Click += new System.EventHandler(this.StartAllButton_Click);
            // 
            // StopAllButton
            // 
            this.StopAllButton.Location = new System.Drawing.Point(238, 272);
            this.StopAllButton.Name = "StopAllButton";
            this.StopAllButton.Size = new System.Drawing.Size(91, 56);
            this.StopAllButton.TabIndex = 14;
            this.StopAllButton.Text = "Stop All";
            this.StopAllButton.UseVisualStyleBackColor = true;
            this.StopAllButton.Click += new System.EventHandler(this.StopAllButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(469, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 40);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 491);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StopAllButton);
            this.Controls.Add(this.StartAllButton);
            this.Controls.Add(this.RunInListButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.StartButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDigest;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.TextBox textBoxUdid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxFolders;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ExtractButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button CheckDupeButton;
        private System.Windows.Forms.Button DeleteDupeButton;
        private System.Windows.Forms.Button RunInListButton;
        private System.Windows.Forms.Button StartAllButton;
        private System.Windows.Forms.Button StopAllButton;
        private System.Windows.Forms.Button SetToAllButton;
        private System.Windows.Forms.Button CheckDupeRangeButton;
        private System.Windows.Forms.Button buttonSetToning0;
        private System.Windows.Forms.Button buttonUtil;
        private System.Windows.Forms.Button buttonUpdateInsuranceUpdate;
        private System.Windows.Forms.Button findLongNum;
        private System.Windows.Forms.Button button1;
    }
}

