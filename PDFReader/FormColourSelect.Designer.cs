namespace WebbIE
{
    partial class FormColourSelect
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
            this.radWindowsDefault = new System.Windows.Forms.RadioButton();
            this.radBlackOnWhite = new System.Windows.Forms.RadioButton();
            this.radWhiteOnBlack = new System.Windows.Forms.RadioButton();
            this.radYellowOnBlack = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radWindowsDefault
            // 
            this.radWindowsDefault.AutoSize = true;
            this.radWindowsDefault.Location = new System.Drawing.Point(15, 16);
            this.radWindowsDefault.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radWindowsDefault.Name = "radWindowsDefault";
            this.radWindowsDefault.Size = new System.Drawing.Size(227, 34);
            this.radWindowsDefault.TabIndex = 0;
            this.radWindowsDefault.TabStop = true;
            this.radWindowsDefault.Text = "Use Windows default";
            this.radWindowsDefault.UseVisualStyleBackColor = true;
            // 
            // radBlackOnWhite
            // 
            this.radBlackOnWhite.AutoSize = true;
            this.radBlackOnWhite.BackColor = System.Drawing.Color.White;
            this.radBlackOnWhite.ForeColor = System.Drawing.Color.Black;
            this.radBlackOnWhite.Location = new System.Drawing.Point(15, 64);
            this.radBlackOnWhite.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radBlackOnWhite.Name = "radBlackOnWhite";
            this.radBlackOnWhite.Size = new System.Drawing.Size(165, 34);
            this.radBlackOnWhite.TabIndex = 1;
            this.radBlackOnWhite.TabStop = true;
            this.radBlackOnWhite.Text = "Black on white";
            this.radBlackOnWhite.UseVisualStyleBackColor = false;
            // 
            // radWhiteOnBlack
            // 
            this.radWhiteOnBlack.AutoSize = true;
            this.radWhiteOnBlack.BackColor = System.Drawing.Color.Black;
            this.radWhiteOnBlack.ForeColor = System.Drawing.Color.White;
            this.radWhiteOnBlack.Location = new System.Drawing.Point(15, 112);
            this.radWhiteOnBlack.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radWhiteOnBlack.Name = "radWhiteOnBlack";
            this.radWhiteOnBlack.Size = new System.Drawing.Size(170, 34);
            this.radWhiteOnBlack.TabIndex = 2;
            this.radWhiteOnBlack.TabStop = true;
            this.radWhiteOnBlack.Text = "White on black";
            this.radWhiteOnBlack.UseVisualStyleBackColor = false;
            // 
            // radYellowOnBlack
            // 
            this.radYellowOnBlack.AutoSize = true;
            this.radYellowOnBlack.BackColor = System.Drawing.Color.Black;
            this.radYellowOnBlack.ForeColor = System.Drawing.Color.Yellow;
            this.radYellowOnBlack.Location = new System.Drawing.Point(15, 160);
            this.radYellowOnBlack.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radYellowOnBlack.Name = "radYellowOnBlack";
            this.radYellowOnBlack.Size = new System.Drawing.Size(175, 34);
            this.radYellowOnBlack.TabIndex = 3;
            this.radYellowOnBlack.TabStop = true;
            this.radYellowOnBlack.Text = "Yellow on black";
            this.radYellowOnBlack.UseVisualStyleBackColor = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(314, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 44);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(314, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 44);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmColourSelect
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(444, 217);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.radYellowOnBlack);
            this.Controls.Add(this.radWhiteOnBlack);
            this.Controls.Add(this.radBlackOnWhite);
            this.Controls.Add(this.radWindowsDefault);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "frmColourSelect";
            this.Text = "Colour";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radWindowsDefault;
        private System.Windows.Forms.RadioButton radBlackOnWhite;
        private System.Windows.Forms.RadioButton radWhiteOnBlack;
        private System.Windows.Forms.RadioButton radYellowOnBlack;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}