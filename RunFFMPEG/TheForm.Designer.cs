
namespace RunFFMPEG
{
    partial class TheForm
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
            this.tbOne = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbOne
            // 
            this.tbOne.AcceptsReturn = true;
            this.tbOne.AcceptsTab = true;
            this.tbOne.AllowDrop = true;
            this.tbOne.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOne.Location = new System.Drawing.Point(0, 0);
            this.tbOne.Multiline = true;
            this.tbOne.Name = "tbOne";
            this.tbOne.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOne.Size = new System.Drawing.Size(800, 450);
            this.tbOne.TabIndex = 1;
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbOne);
            this.Name = "TheForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOne;
    }
}

