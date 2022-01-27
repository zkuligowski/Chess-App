namespace WinFormsApp1
{
    partial class Chess_App
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_From_File = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_Analyze = new System.Windows.Forms.Button();
            this.button_Clear_White_Chess_Notation = new System.Windows.Forms.Button();
            this.listBox_White_Chess_Notation = new System.Windows.Forms.ListBox();
            this.listBox_Black_Chess_Notation = new System.Windows.Forms.ListBox();
            this.textBox_Path = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.button_Clear_Black_Chess_Notation = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(1, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // button_From_File
            // 
            this.button_From_File.Location = new System.Drawing.Point(585, 12);
            this.button_From_File.Name = "button_From_File";
            this.button_From_File.Size = new System.Drawing.Size(75, 23);
            this.button_From_File.TabIndex = 3;
            this.button_From_File.Text = "From File";
            this.button_From_File.UseVisualStyleBackColor = true;
            this.button_From_File.Click += new System.EventHandler(this.button_From_File_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(687, 12);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(75, 23);
            this.button_Clear.TabIndex = 4;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_Analyze
            // 
            this.button_Analyze.Location = new System.Drawing.Point(797, 11);
            this.button_Analyze.Name = "button_Analyze";
            this.button_Analyze.Size = new System.Drawing.Size(75, 23);
            this.button_Analyze.TabIndex = 5;
            this.button_Analyze.Text = "Analyze";
            this.button_Analyze.UseVisualStyleBackColor = true;
            this.button_Analyze.Click += new System.EventHandler(this.button_Analyze_Click);
            // 
            // button_Clear_White_Chess_Notation
            // 
            this.button_Clear_White_Chess_Notation.Location = new System.Drawing.Point(971, 3);
            this.button_Clear_White_Chess_Notation.Name = "button_Clear_White_Chess_Notation";
            this.button_Clear_White_Chess_Notation.Size = new System.Drawing.Size(104, 49);
            this.button_Clear_White_Chess_Notation.TabIndex = 6;
            this.button_Clear_White_Chess_Notation.Text = "Clear White Chess Notation";
            this.button_Clear_White_Chess_Notation.UseVisualStyleBackColor = true;
            this.button_Clear_White_Chess_Notation.Click += new System.EventHandler(this.button_Clear_White_Chess_Notation_Click);
            // 
            // listBox_White_Chess_Notation
            // 
            this.listBox_White_Chess_Notation.FormattingEnabled = true;
            this.listBox_White_Chess_Notation.ItemHeight = 15;
            this.listBox_White_Chess_Notation.Location = new System.Drawing.Point(873, 58);
            this.listBox_White_Chess_Notation.Name = "listBox_White_Chess_Notation";
            this.listBox_White_Chess_Notation.Size = new System.Drawing.Size(457, 274);
            this.listBox_White_Chess_Notation.TabIndex = 7;
            this.listBox_White_Chess_Notation.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox_Black_Chess_Notation
            // 
            this.listBox_Black_Chess_Notation.FormattingEnabled = true;
            this.listBox_Black_Chess_Notation.ItemHeight = 15;
            this.listBox_Black_Chess_Notation.Location = new System.Drawing.Point(1358, 58);
            this.listBox_Black_Chess_Notation.Name = "listBox_Black_Chess_Notation";
            this.listBox_Black_Chess_Notation.Size = new System.Drawing.Size(406, 274);
            this.listBox_Black_Chess_Notation.TabIndex = 8;
            this.listBox_Black_Chess_Notation.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // textBox_Path
            // 
            this.textBox_Path.Location = new System.Drawing.Point(12, 12);
            this.textBox_Path.Name = "textBox_Path";
            this.textBox_Path.Size = new System.Drawing.Size(432, 23);
            this.textBox_Path.TabIndex = 9;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Black;
            this.pictureBox2.Location = new System.Drawing.Point(807, 383);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(800, 600);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Black;
            this.pictureBox3.Location = new System.Drawing.Point(807, 361);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(800, 600);
            this.pictureBox3.TabIndex = 11;
            this.pictureBox3.TabStop = false;
            // 
            // button_Clear_Black_Chess_Notation
            // 
            this.button_Clear_Black_Chess_Notation.Location = new System.Drawing.Point(1326, 3);
            this.button_Clear_Black_Chess_Notation.Name = "button_Clear_Black_Chess_Notation";
            this.button_Clear_Black_Chess_Notation.Size = new System.Drawing.Size(104, 49);
            this.button_Clear_Black_Chess_Notation.TabIndex = 12;
            this.button_Clear_Black_Chess_Notation.Text = "Clear Black Chess Notation";
            this.button_Clear_Black_Chess_Notation.UseVisualStyleBackColor = true;
            this.button_Clear_Black_Chess_Notation.Click += new System.EventHandler(this.button_Clear_Black_Chess_Notation_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(450, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(298, 657);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 99);
            this.button2.TabIndex = 14;
            this.button2.Text = "testowy";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(24, 657);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(247, 304);
            this.listBox1.TabIndex = 15;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged_1);
            // 
            // Chess_App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1776, 973);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_Clear_Black_Chess_Notation);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.textBox_Path);
            this.Controls.Add(this.listBox_Black_Chess_Notation);
            this.Controls.Add(this.listBox_White_Chess_Notation);
            this.Controls.Add(this.button_Clear_White_Chess_Notation);
            this.Controls.Add(this.button_Analyze);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_From_File);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Chess_App";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button_From_File;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Button button_Analyze;
        private System.Windows.Forms.Button button_Clear_White_Chess_Notation;
        private System.Windows.Forms.ListBox listBox_White_Chess_Notation;
        private System.Windows.Forms.ListBox listBox_Black_Chess_Notation;
        private System.Windows.Forms.TextBox textBox_Path;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button button_Clear_Black_Chess_Notation;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
    }
}
