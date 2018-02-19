using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebbIE
{
    /// <summary>
    /// The colours used in UI components like text fields - a pair of colours, for text and background.
    /// </summary>
    public enum ColourScheme
    {
        /// <summary>
        /// Windows default.
        /// </summary>
        WindowsDefault = 0,
        /// <summary>
        /// Black on white.
        /// </summary>
        BlackOnWhite,
        /// <summary>
        /// White on black.
        /// </summary>
        WhiteOnBlack,
        /// <summary>
        /// Yellow on black.
        /// </summary>
        YellowOnBlack
    }


    /// <summary>
    /// A form for selecting the colour used in client areas (e.g. text areas)
    /// </summary>
    public partial class FormColourSelect : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public FormColourSelect()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Goes through all the relevant controls on Form f, changing the colour scheme to cs.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="cs"></param>
        public static void SetColourScheme(System.Windows.Forms.Form f, ColourScheme cs)
        {
            System.Drawing.Color fc;
            System.Drawing.Color bc;
            switch (cs)
            {
                case ColourScheme.BlackOnWhite:
                    fc= System.Drawing.Color.Black;
                    bc= System.Drawing.Color.White;
                    break;
                case ColourScheme.WhiteOnBlack:
                    fc = System.Drawing.Color.White;
                    bc = System.Drawing.Color.Black;
                    break;
                case ColourScheme.YellowOnBlack:
                    fc = System.Drawing.Color.Yellow;
                    bc = System.Drawing.Color.Black;
                    break;
                default: // ColourScheme.WindowsDefault: 
                    fc = System.Drawing.Color.FromName(System.Drawing.KnownColor.WindowText.ToString());
                    bc = System.Drawing.Color.FromName(System.Drawing.KnownColor.Window.ToString());
                    break;
            }
            foreach (System.Windows.Forms.Control c in f.Controls)
            {
                switch (c.GetType().Name)
                {
                    case "RichTextBox":
                        c.ForeColor = fc;
                        c.BackColor = bc;
                        break;
                }
            }

        }

        /// <summary>
        /// Sets the radio selection to the current colour scheme value.
        /// </summary>
        /// <param name="colourScheme"></param>
        public void SetCurrentSelection(int colourScheme)
        {
            WebbIE.ColourScheme cs = (WebbIE.ColourScheme)colourScheme;
            switch (cs)
            {
                case ColourScheme.BlackOnWhite:
                    this.radBlackOnWhite.Checked = true;
                    break;
                case ColourScheme.WhiteOnBlack:
                    this.radWhiteOnBlack.Checked = true;
                    break;
                case ColourScheme.YellowOnBlack:
                    this.radYellowOnBlack.Checked = true;
                    break;
                default:
                    this.radWindowsDefault.Checked = true;
                    break;
            }
        }

        /// <summary>
        /// The selected colour scheme from the dialogue.
        /// </summary>
        public ColourScheme colourScheme
        {
            get
            {
                if (this.radYellowOnBlack.Checked)
                {
                    return ColourScheme.YellowOnBlack;
                } 
                else if (this.radWhiteOnBlack.Checked)
                {
                    return ColourScheme.WhiteOnBlack;
                }
                else if (this.radBlackOnWhite.Checked)
                {
                    return ColourScheme.BlackOnWhite;
                }
                else
                {
                    return ColourScheme.WindowsDefault;
                }
                
            }
                
        }
        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

    }
}
