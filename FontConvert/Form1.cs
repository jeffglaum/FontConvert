using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using FontConvert;


namespace WindowsFormsApplication1
{
    public partial class FontConvertForm : Form
    {

        List<char> charList = null;

        bool useNarrowChar  = false;
        int charWidthNarrow = 8;
        int charWidthWide   = 16;
        int charHeight      = 19;
        Font charFont       = null;


        private const int LR_LOADFROMFILE = 0x0010;
        private const int LR_MONOCHROME = 0x0001;

        public FontConvertForm()
        {
            InitializeComponent();

            charFont = this.Font;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream fileStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((fileStream = openFileDialog.OpenFile()) != null)
                    {
                        using (var fs = fileStream)
                        {
                            // Detect BOM to choose encoding. If no BOM is present fall back to system ANSI (Encoding.Default).
                            byte[] bom = new byte[4];
                            int bytesRead = fs.Read(bom, 0, 4);
                            Encoding fileEncoding;
                            int bomLength = 0;

                            if (bytesRead >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                            {
                                fileEncoding = Encoding.UTF8;
                                bomLength = 3;
                            }
                            else if (bytesRead >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
                            {
                                fileEncoding = Encoding.Unicode; // UTF-16 LE
                                bomLength = 2;
                            }
                            else if (bytesRead >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
                            {
                                fileEncoding = Encoding.BigEndianUnicode; // UTF-16 BE
                                bomLength = 2;
                            }
                            else
                            {
                                // No BOM detected - assume ANSI (system default code page)
                                fileEncoding = Encoding.Default;
                                bomLength = 0;
                            }

                            // Reposition stream after BOM (if any) so we don't include BOM characters in the reader output.
                            fs.Position = bomLength;

                            using (StreamReader reader = new StreamReader(fs, fileEncoding))
                            {
                                List<char> tempList = new List<char>();

                                while (reader.Peek() >= 0)
                                {
                                    tempList.Add((char)reader.Read());
                                }

                                charList = tempList.Distinct().ToList();
                                charList.Remove('\n');
                                charList.Remove('\r');
                                charList.Sort();

                                // StreamReader and FileStream are disposed by the using blocks.
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: Failed to read file from disk.  Original error: " + ex.Message);
                }
            }

            this.Refresh();

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            DialogResult result = fontDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get Font.
                charFont = fontDialog.Font;

                this.Refresh();
            }
        }

        private void FontConvertForm_Paint(object sender, PaintEventArgs e)
        {
            int gridWidth = 0;
            int gridHeight = 0;

            System.Drawing.Pen outlinePen = new System.Drawing.Pen(System.Drawing.Color.LightBlue);
            System.Drawing.Pen splitlinePen = new System.Drawing.Pen(System.Drawing.Color.LightGray);

            System.Drawing.Graphics formGraphics = this.CreateGraphics();

            if (useNarrowChar == true)
                gridWidth = charWidthNarrow + 1;
            else
                gridWidth = charWidthWide + 1;
            gridHeight = charHeight + 1;

            bool drawSplitline = false;
            for (int x = 0 ; x <= this.Width; x += gridWidth)
            {
              //  if (useNarrowChar == false && drawSplitline == true)
              //      formGraphics.DrawLine(splitlinePen, x, 0, x, this.Height);
              //  else
                    formGraphics.DrawLine(outlinePen, x, 0, x, this.Height);

                drawSplitline = !drawSplitline;
            }
           
            for (int y = this.MainMenuStrip.Height ; y <= this.Height ; y += gridHeight)
            {
                formGraphics.DrawLine(outlinePen, 0, y, this.Width, y);
            }
            
            outlinePen.Dispose();
            splitlinePen.Dispose();
            formGraphics.Dispose();

            int charX = 1;
            int charY = this.MainMenuStrip.Height + 1;
            int charWidth = ((useNarrowChar == true) ? charWidthNarrow : charWidthWide);
            
            TextFormatFlags textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            if (charList != null)
            {
                foreach (char Character in charList)
                {
                    TextRenderer.DrawText(e.Graphics, Character.ToString(), charFont, new Rectangle(charX, charY, charWidth, charHeight), SystemColors.ControlText, textFlags);
                    charX += (charWidth + 1);
                }
            }
        }

        private void narrowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useNarrowChar = true;
            this.Refresh();
        }

        private void wideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useNarrowChar = false;
            this.Refresh();
        }

        protected void SetIndexedPixel(int x, int y, BitmapData bmd, bool pixel)
        {
            int index = y * bmd.Stride + (x >> 3);
            byte p = Marshal.ReadByte(bmd.Scan0, index);
            byte mask = (byte)(0x80 >> (x & 0x7));

            if (pixel)
                p |= mask;
            else
                p &= (byte)(mask ^ 0xff);

            Marshal.WriteByte(bmd.Scan0, index, p);
        }


        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte glyphCol1 = 0;
            byte glyphCol2 = 0;
            List<byte> listCol1 = new List<byte>();
            List<byte> listCol2 = new List<byte>();

            // Nothing to do...
            //
            if (charList == null)
                return;

            int charWidth = ((useNarrowChar == true) ? charWidthNarrow : charWidthWide);
            
            // Create a new bitmap.
            //
            var bmpScreenshot = new Bitmap(charWidth,
                                           charHeight,
                                           PixelFormat.Format32bppArgb);
                
            // Create a graphics object from the bitmap.
            //
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Prepare the glyph output window but don't show it yet — showing it here
            // can cover the characters on the form and cause subsequent screenshots
            // to capture the output window instead of the glyphs. Show it after
            // all screenshots are taken and the text is populated.
            GlyphOutput outputWindow = new GlyphOutput();

            
            // OUTPUT: Character Count
            if (useNarrowChar == true)
            {
                outputWindow.glyphText.Text = "  " + charList.Count.ToString() + ",\t\t// Number of Narrow glyphs.\r\n";
                outputWindow.glyphText.Text += "  0,\t\t// Number of Wide glyphs.\r\n";
            }
            else
            {
                outputWindow.glyphText.Text = "  0,\t\t// Number of Narrow glyphs.\r\n";
                outputWindow.glyphText.Text += "  " + charList.Count.ToString() + ",\t\t// Number of Wide glyphs.\r\n";
                outputWindow.glyphText.Text += "  {\t\t// Narrow glyphs.\r\n";
                outputWindow.glyphText.Text += "    {\r\n";
                outputWindow.glyphText.Text += "      0x0000,\r\n";
                outputWindow.glyphText.Text += "      0x00,\r\n";
                outputWindow.glyphText.Text += "      {\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00,\r\n";
                outputWindow.glyphText.Text += "        0x00\r\n";
                outputWindow.glyphText.Text += "      }\r\n";
                outputWindow.glyphText.Text += "    }\r\n";
                outputWindow.glyphText.Text += "  },\r\n";
                outputWindow.glyphText.Text += "  {\t\t// Wide glyphs.\r\n";
            }


            int captureX = this.PointToScreen(new Point(0, 0)).X + 1;
            int captureY = this.PointToScreen(new Point(0, 0)).Y + this.MainMenuStrip.Height + 1;

            // Ensure the form is up-to-date on screen before taking screenshots.
            this.Refresh();
            Application.DoEvents();

            TextFormatFlags textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            foreach (char Character in charList)
            {

                // Render the character directly onto the in-memory bitmap instead of
                // capturing the screen. CopyFromScreen is fragile (focus, scaling,
                // occlusion) and can produce empty/blank captures which resulted in
                // many zero glyphs for narrow mode. Clearing and drawing the text
                // ensures consistent results.
                gfxScreenshot.Clear(SystemColors.Control);
                TextRenderer.DrawText(gfxScreenshot, Character.ToString(), charFont, new Rectangle(0, 0, charWidth, charHeight), SystemColors.ControlText, textFlags);

                // Save the screenshot to the specified path that the user has chosen.
                //bmpScreenshot.Save("c:\\temp\\orig.bmp", ImageFormat.Bmp);


                // Lock the bits of the original bitmap
                //
                BitmapData bmdo = bmpScreenshot.LockBits(new Rectangle(0, 0, bmpScreenshot.Width, bmpScreenshot.Height), ImageLockMode.ReadOnly, bmpScreenshot.PixelFormat);

                // Also lock the new 1bpp bitmap
                //
                Bitmap bm = new Bitmap(charWidth, charHeight, PixelFormat.Format1bppIndexed);
                BitmapData bmdn = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

                // Loop here for all chars.
                listCol1.Clear();
                listCol2.Clear();
                
                // OUTPUT: Unicode Font Weight
                outputWindow.glyphText.Text += "    {\r\n";
                string fontWeight = Convert.ToInt32(Character).ToString("x4");
                outputWindow.glyphText.Text += "      0x" + fontWeight + ",\t// Character: " + Character.ToString() + "\r\n";
                
                // OUTPUT: Font Attribute
                outputWindow.glyphText.Text += "      0x00,\r\n";
                outputWindow.glyphText.Text += "      {\r\n";

                // Scan through the pixels Y by X
                //
                int x, y;
                for (y = 0; y < bmpScreenshot.Height; y++)
                {
                    for (x = 0, glyphCol1 = 0, glyphCol2 = 0; x < bmpScreenshot.Width; x++)
                    {
                        // Generate the address of the color pixel
                        //
                        int index = y * bmdo.Stride + (x * 4);

                        // Check its brightness
                        //
                        if (Color.FromArgb(Marshal.ReadByte(bmdo.Scan0, index + 2),
                                           Marshal.ReadByte(bmdo.Scan0, index + 1),
                                           Marshal.ReadByte(bmdo.Scan0, index)).GetBrightness() > 0.5f)
                        {
                            this.SetIndexedPixel(x, y, bmdn, true); //set it if its bright.
                        }
                        else
                        {
                            if (x >= charWidthNarrow)
                                glyphCol2 |= (byte)(1 << ((charWidthNarrow - 1) - (x - charWidthNarrow)));
                            else
                                glyphCol1 |= (byte)(1 << (charWidthNarrow - 1 - x));
                        }
                    }

                    listCol1.Add(glyphCol1);
                    if (useNarrowChar == false)
                    {
                        listCol2.Add(glyphCol2);
                    }
                }
                
                // OUTPUT: Column 1
                int lineCount = bmpScreenshot.Height;
                foreach (byte glyph in listCol1)
                {
                    outputWindow.glyphText.Text += "        0x" + glyph.ToString("X2");
                    if (--lineCount != 0)
                    {
                        outputWindow.glyphText.Text += ",";
                    }
                    outputWindow.glyphText.Text += "\r\n";
                }

                if (useNarrowChar == false)
                {
                    outputWindow.glyphText.Text += "      },\r\n";
                    outputWindow.glyphText.Text += "      {\r\n";

                    // OUTPUT: Column 2
                    lineCount = bmpScreenshot.Height;
                    foreach (byte glyph in listCol2)
                    {
                        outputWindow.glyphText.Text += "        0x" + glyph.ToString("X2");
                        if (--lineCount != 0)
                        {
                            outputWindow.glyphText.Text += ",";
                        }
                        outputWindow.glyphText.Text += "\r\n";
                    }
                }
                
                outputWindow.glyphText.Text += "      }\r\n";            
                outputWindow.glyphText.Text += "    },\r\n";

                // Clean-up
                //
                bm.UnlockBits(bmdn);
                bmpScreenshot.UnlockBits(bmdo);
                
                
                // Save the 1bpp screenshot and free.
                //
                //bm.Save("c:\\temp\\final" + Character.ToString() +".bmp", ImageFormat.Bmp);
                bm.Dispose();

                // Increment to next character
                //
                captureX += (charWidth + 1);
            }

            outputWindow.glyphText.Text += "  }\r\n";

            // Dispose the graphics object created from the bitmap.
            gfxScreenshot.Dispose();

            // Now show the output window after all capture and text generation is complete.
            outputWindow.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }
    }
}
