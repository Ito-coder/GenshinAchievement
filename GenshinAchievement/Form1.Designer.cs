namespace GenshinAchievement
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            textBox2 = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            checkBox1 = new CheckBox();
            listView1 = new ListView();
            pictureBox2 = new PictureBox();
            textBox1 = new TextBox();
            textBox3 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            checkBox2 = new CheckBox();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(451, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(337, 714);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(239, 26);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(206, 121);
            textBox2.TabIndex = 3;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(239, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(68, 19);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "キャプチャ";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // listView1
            // 
            listView1.CheckBoxes = true;
            listView1.Location = new Point(1, 1);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.ShowGroups = false;
            listView1.Size = new Size(232, 764);
            listView1.TabIndex = 5;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.SmallIcon;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.MouseClick += listView1_MouseClick;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(239, 153);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(206, 146);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 6;
            pictureBox2.TabStop = false;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(241, 538);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(204, 227);
            textBox1.TabIndex = 7;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(239, 336);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(206, 196);
            textBox3.TabIndex = 8;
            // 
            // button1
            // 
            button1.Location = new Point(239, 305);
            button1.Name = "button1";
            button1.Size = new Size(104, 25);
            button1.TabIndex = 9;
            button1.Text = "取得済みに変更";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(396, 310);
            button2.Name = "button2";
            button2.Size = new Size(35, 20);
            button2.TabIndex = 10;
            button2.Text = "test";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            button2.Click += button2_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Enabled = false;
            checkBox2.Location = new Point(313, 2);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(50, 19);
            checkBox2.TabIndex = 11;
            checkBox2.Text = "選択";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(396, 0);
            button3.Name = "button3";
            button3.Size = new Size(46, 20);
            button3.TabIndex = 12;
            button3.Text = "save";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(730, 738);
            button4.Name = "button4";
            button4.Size = new Size(58, 27);
            button4.TabIndex = 13;
            button4.Text = "初期化";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 766);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(checkBox2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox3);
            Controls.Add(textBox1);
            Controls.Add(pictureBox2);
            Controls.Add(listView1);
            Controls.Add(checkBox1);
            Controls.Add(textBox2);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox textBox2;
        private System.Windows.Forms.Timer timer1;
        private CheckBox checkBox1;
        private ListView listView1;
        private PictureBox pictureBox2;
        private TextBox textBox1;
        private TextBox textBox3;
        private Button button1;
        private Button button2;
        private CheckBox checkBox2;
        private Button button3;
        private Button button4;
    }
}