/// <summary>
/// Convenience class for hosting SetLineSpacing function.
/// </summary>
public class RichTextBoxLineSpacing
{
    /// <summary>
    /// SendMessage. Used in this case to do line spacing in the rich text box.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    [System.Runtime.InteropServices.DllImport("user32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern System.IntPtr SendMessage(System.Runtime.InteropServices.HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);

    /// <summary>
    /// PARAFORMAT is used to do linespacing in RichTextBoxes
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct PARAFORMAT
    {
        public int cbSize;
        public uint dwMask;
        public short wNumbering;
        public short wReserved;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;
        // PARAFORMAT2 from here onwards
        public int dySpaceBefore;
        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public short wShadingWeight;
        public short wShadingStyle;
        public short wNumberingStart;
        public short wNumberingStyle;
        public short wNumberingTab;
        public short wBorderSpace;
        public short wBorderWidth;
        public short wBorders;
    }

    /// <summary>
    /// Tell SendMessage to change line spacing
    /// </summary>
    private const int PFM_LINESPACING = 0x00000100;
    private const int SCF_SELECTION = 1;

    /// <summary>
    /// Tell SendMessage to change paragraph formatting.
    /// Used for line spacing.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb774276(v=vs.85).aspx
    /// </summary>
    private const int EM_SETPARAFORMAT = 1095;

    /// <summary>
    /// Line spacing in rich text boxes.
    /// Type of line spacing. To use this member, set the PFM_SPACEAFTER flag in the dwMask member. This member can be one of the following values.
    /// </summary>
    public enum SpacingRule
    {
        /// <summary>
        /// Single spacing. The dyLineSpacing member is ignored.
        /// </summary>
        SingleSpacing = 0,

        /// <summary>
        /// One-and-a-half spacing. The dyLineSpacing member is ignored.
        /// </summary>
        OneAndAHalfSpacing = 1,

        /// <summary>
        /// Double spacing. The dyLineSpacing member is ignored.
        /// </summary>
        DoubleSpacing = 2,

        /// <summary>
        /// The dyLineSpacing member specifies the spacingfrom one line to the next, in twips. However, if dyLineSpacing specifies a value that is less than single spacing, the control displays single-spaced text.
        /// </summary>
        UserDefinedMiniumumOne = 3,
    
        /// <summary>
        /// The dyLineSpacing member specifies the spacing from one line to the next, in twips. The control uses the exact spacing specified, even if dyLineSpacing specifies a value that is less than single spacing.
        /// </summary>
        UserDefinedNoMinimum = 4,

        /// <summary>
        /// The value of dyLineSpacing / 20 is the spacing, in lines, from one line to the next. Thus, setting dyLineSpacing to 20 produces single-spaced text, 40 is double spaced, 60 is triple spaced, and so on. 
        /// </summary>
        UserDefinedInTwentieths = 5
    }

    /// <summary>
    /// Call this with the line spacing you want in the RichTextBox rtb.
    /// See SpacingRule for the options.
    /// </summary>
    /// <param name="rule"></param>
    /// <param name="space"></param>
    /// <param name="rtb"></param>
    public static void SetLineSpacing(SpacingRule rule, int space, System.Windows.Forms.RichTextBox rtb)
    {
        PARAFORMAT fmt = new PARAFORMAT();
        fmt.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(fmt);
        fmt.dwMask = PFM_LINESPACING;
        fmt.dyLineSpacing = space;
        fmt.bLineSpacingRule = (byte)rule;
        rtb.SelectAll();
        SendMessage(new System.Runtime.InteropServices.HandleRef(rtb, rtb.Handle),
                     EM_SETPARAFORMAT,
                     SCF_SELECTION,
                     ref fmt
                   );
    }
}