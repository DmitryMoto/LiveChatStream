namespace LiveChatStream
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.YouTube_checkBox = new System.Windows.Forms.CheckBox();
            this.Twitch_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(165, 601);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(531, 368);
            this.listBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(246, 601);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // YouTube_checkBox
            // 
            this.YouTube_checkBox.AutoSize = true;
            this.YouTube_checkBox.Location = new System.Drawing.Point(681, 12);
            this.YouTube_checkBox.Name = "YouTube_checkBox";
            this.YouTube_checkBox.Size = new System.Drawing.Size(70, 17);
            this.YouTube_checkBox.TabIndex = 3;
            this.YouTube_checkBox.Text = "YouTube";
            this.YouTube_checkBox.UseVisualStyleBackColor = true;
            // 
            // Twitch_checkBox
            // 
            this.Twitch_checkBox.AutoSize = true;
            this.Twitch_checkBox.Location = new System.Drawing.Point(681, 35);
            this.Twitch_checkBox.Name = "Twitch_checkBox";
            this.Twitch_checkBox.Size = new System.Drawing.Size(58, 17);
            this.Twitch_checkBox.TabIndex = 4;
            this.Twitch_checkBox.Text = "Twitch";
            this.Twitch_checkBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 636);
            this.Controls.Add(this.Twitch_checkBox);
            this.Controls.Add(this.YouTube_checkBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox YouTube_checkBox;
        private System.Windows.Forms.CheckBox Twitch_checkBox;
    }
}

