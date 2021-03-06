﻿namespace Launcher
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnValidate = new System.Windows.Forms.Button();
            this.BtnRun = new System.Windows.Forms.Button();
            this.BtnFile = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.openFD = new System.Windows.Forms.OpenFileDialog();
            this.lblLandis = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblWww = new System.Windows.Forms.LinkLabel();
            this.TxtBoxStatus = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnLogFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.Location = new System.Drawing.Point(685, 362);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 25);
            this.BtnClose.TabIndex = 10;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnValidate
            // 
            this.BtnValidate.Enabled = false;
            this.BtnValidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnValidate.Location = new System.Drawing.Point(442, 362);
            this.BtnValidate.Name = "BtnValidate";
            this.BtnValidate.Size = new System.Drawing.Size(75, 25);
            this.BtnValidate.TabIndex = 9;
            this.BtnValidate.Text = "Validate";
            this.BtnValidate.UseVisualStyleBackColor = true;
            this.BtnValidate.Click += new System.EventHandler(this.BtnValidate_Click);
            // 
            // BtnRun
            // 
            this.BtnRun.Enabled = false;
            this.BtnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRun.Location = new System.Drawing.Point(604, 362);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(75, 25);
            this.BtnRun.TabIndex = 8;
            this.BtnRun.Text = "Run";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // BtnFile
            // 
            this.BtnFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnFile.Location = new System.Drawing.Point(682, 68);
            this.BtnFile.Name = "BtnFile";
            this.BtnFile.Size = new System.Drawing.Size(79, 23);
            this.BtnFile.TabIndex = 12;
            this.BtnFile.Text = "Browse ...";
            this.BtnFile.UseVisualStyleBackColor = true;
            this.BtnFile.Click += new System.EventHandler(this.BtnFile_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(106, 67);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(570, 22);
            this.txtFilePath.TabIndex = 11;
            // 
            // openFD
            // 
            this.openFD.FileName = "openFileDialog1";
            // 
            // lblLandis
            // 
            this.lblLandis.AutoSize = true;
            this.lblLandis.Font = new System.Drawing.Font("Verdana", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLandis.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblLandis.Location = new System.Drawing.Point(225, -3);
            this.lblLandis.Name = "lblLandis";
            this.lblLandis.Size = new System.Drawing.Size(225, 42);
            this.lblLandis.TabIndex = 13;
            this.lblLandis.Text = "LANDIS-II";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.ForestGreen;
            this.label1.Location = new System.Drawing.Point(200, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 25);
            this.label1.TabIndex = 14;
            this.label1.Text = "Forest Landscape Model";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "Scenario file";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.ForestGreen;
            this.label3.Location = new System.Drawing.Point(7, 367);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "For more information visit: ";
            // 
            // lblWww
            // 
            this.lblWww.AutoSize = true;
            this.lblWww.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWww.Location = new System.Drawing.Point(163, 367);
            this.lblWww.Name = "lblWww";
            this.lblWww.Size = new System.Drawing.Size(143, 16);
            this.lblWww.TabIndex = 17;
            this.lblWww.TabStop = true;
            this.lblWww.Text = "http://www.landis-ii.org/";
            this.lblWww.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblWww_LinkClicked);
            // 
            // TxtBoxStatus
            // 
            this.TxtBoxStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBoxStatus.Location = new System.Drawing.Point(10, 109);
            this.TxtBoxStatus.Multiline = true;
            this.TxtBoxStatus.Name = "TxtBoxStatus";
            this.TxtBoxStatus.ReadOnly = true;
            this.TxtBoxStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtBoxStatus.Size = new System.Drawing.Size(475, 240);
            this.TxtBoxStatus.TabIndex = 18;
            this.TxtBoxStatus.Text = "Ready ...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Current status";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(491, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(270, 240);
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // BtnLogFile
            // 
            this.BtnLogFile.Enabled = false;
            this.BtnLogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLogFile.Location = new System.Drawing.Point(523, 362);
            this.BtnLogFile.Name = "BtnLogFile";
            this.BtnLogFile.Size = new System.Drawing.Size(75, 25);
            this.BtnLogFile.TabIndex = 21;
            this.BtnLogFile.Text = "Log File";
            this.BtnLogFile.UseVisualStyleBackColor = true;
            this.BtnLogFile.Click += new System.EventHandler(this.BtnLogFile_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 395);
            this.Controls.Add(this.BtnLogFile);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TxtBoxStatus);
            this.Controls.Add(this.lblWww);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLandis);
            this.Controls.Add(this.BtnFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnValidate);
            this.Controls.Add(this.BtnRun);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Launcher";
            this.Text = "LANDIS-II Scenario Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnValidate;
        private System.Windows.Forms.Button BtnRun;
        private System.Windows.Forms.Button BtnFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.OpenFileDialog openFD;
        private System.Windows.Forms.Label lblLandis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel lblWww;
        private System.Windows.Forms.TextBox TxtBoxStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button BtnLogFile;
    }
}

