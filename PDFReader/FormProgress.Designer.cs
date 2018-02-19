namespace PDFReader
{
    partial class FormProgress
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgress));
            this.lblOpening = new System.Windows.Forms.Label();
            this.prgOpening = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tmrProgressUpdater = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblOpening
            // 
            this.lblOpening.AutoSize = true;
            this.lblOpening.Location = new System.Drawing.Point(22, 20);
            this.lblOpening.Name = "lblOpening";
            this.lblOpening.Size = new System.Drawing.Size(214, 59);
            this.lblOpening.TabIndex = 0;
            this.lblOpening.Text = "Opening...";
            // 
            // prgOpening
            // 
            this.prgOpening.Location = new System.Drawing.Point(32, 94);
            this.prgOpening.Name = "prgOpening";
            this.prgOpening.Size = new System.Drawing.Size(898, 75);
            this.prgOpening.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgOpening.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(701, 202);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(229, 91);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tmrProgressUpdater
            // 
            this.tmrProgressUpdater.Enabled = true;
            this.tmrProgressUpdater.Interval = 200;
            this.tmrProgressUpdater.Tick += new System.EventHandler(this.tmrProgressUpdater_Tick);
            // 
            // FormProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(24F, 59F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(958, 314);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.prgOpening);
            this.Controls.Add(this.lblOpening);
            this.Font = new System.Drawing.Font("Segoe UI", 16.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Opening file...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOpening;
        private System.Windows.Forms.ProgressBar prgOpening;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer tmrProgressUpdater;
    }
}