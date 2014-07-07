namespace Replicator
{
    partial class ReplicatorStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplicatorStatus));
            this.TxtBoxStatus = new System.Windows.Forms.TextBox();
            this.BtnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TxtBoxStatus
            // 
            this.TxtBoxStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBoxStatus.Location = new System.Drawing.Point(11, 12);
            this.TxtBoxStatus.Multiline = true;
            this.TxtBoxStatus.Name = "TxtBoxStatus";
            this.TxtBoxStatus.ReadOnly = true;
            this.TxtBoxStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtBoxStatus.Size = new System.Drawing.Size(475, 374);
            this.TxtBoxStatus.TabIndex = 19;
            this.TxtBoxStatus.TabStop = false;
            this.TxtBoxStatus.Text = resources.GetString("TxtBoxStatus.Text");
            // 
            // BtnClose
            // 
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.Location = new System.Drawing.Point(411, 397);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 28;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            // 
            // ReplicatorStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 427);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.TxtBoxStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReplicatorStatus";
            this.Text = "LANDIS-II Scenario Replicator status window";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtBoxStatus;
        private System.Windows.Forms.Button BtnClose;
    }
}