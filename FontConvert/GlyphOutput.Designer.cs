namespace FontConvert
{
    partial class GlyphOutput
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
            this.glyphText = new System.Windows.Forms.TextBox();
            this.glyphOutput_OK = new System.Windows.Forms.Button();
            this.glyphOutput_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // glyphText
            // 
            this.glyphText.Location = new System.Drawing.Point(0, 0);
            this.glyphText.Multiline = true;
            this.glyphText.Name = "glyphText";
            this.glyphText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.glyphText.Size = new System.Drawing.Size(514, 674);
            this.glyphText.TabIndex = 0;
            // 
            // glyphOutput_OK
            // 
            this.glyphOutput_OK.Location = new System.Drawing.Point(426, 682);
            this.glyphOutput_OK.Name = "glyphOutput_OK";
            this.glyphOutput_OK.Size = new System.Drawing.Size(75, 23);
            this.glyphOutput_OK.TabIndex = 1;
            this.glyphOutput_OK.Text = "&OK";
            this.glyphOutput_OK.UseVisualStyleBackColor = true;
            this.glyphOutput_OK.Click += new System.EventHandler(this.glyphOutput_OK_Click);
            // 
            // glyphOutput_Save
            // 
            this.glyphOutput_Save.Location = new System.Drawing.Point(12, 682);
            this.glyphOutput_Save.Name = "glyphOutput_Save";
            this.glyphOutput_Save.Size = new System.Drawing.Size(75, 23);
            this.glyphOutput_Save.TabIndex = 2;
            this.glyphOutput_Save.Text = "&Save to File";
            this.glyphOutput_Save.UseVisualStyleBackColor = true;
            // 
            // GlyphOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 717);
            this.Controls.Add(this.glyphOutput_Save);
            this.Controls.Add(this.glyphOutput_OK);
            this.Controls.Add(this.glyphText);
            this.Name = "GlyphOutput";
            this.Text = "GlyphOutput";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox glyphText;
        private System.Windows.Forms.Button glyphOutput_OK;
        private System.Windows.Forms.Button glyphOutput_Save;
    }
}