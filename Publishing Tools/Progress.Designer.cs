namespace Publishing_Tools
{
    partial class Progress
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
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(47, 28);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(464, 277);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(226, 330);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(102, 35);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "OK";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // BackupProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 386);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.logBox);
            this.Name = "BackupProgress";
            this.Text = "BackupProgress";
            this.Load += new System.EventHandler(this.BackupProgress_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button exitButton;
    }
}