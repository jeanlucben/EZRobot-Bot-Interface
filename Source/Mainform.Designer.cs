namespace SynbotPlugin
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.botConsole = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.botInput = new System.Windows.Forms.TextBox();
            this.textBoxUserid = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ucConfigurationButton1 = new EZ_Builder.UCForms.UC.UCConfigurationButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // botConsole
            // 
            this.botConsole.BackColor = System.Drawing.Color.Black;
            this.botConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.botConsole.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botConsole.ForeColor = System.Drawing.Color.Lime;
            this.botConsole.Location = new System.Drawing.Point(0, 0);
            this.botConsole.Multiline = true;
            this.botConsole.Name = "botConsole";
            this.botConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.botConsole.Size = new System.Drawing.Size(745, 248);
            this.botConsole.TabIndex = 0;
            this.botConsole.Text = "This is the Syn Bot Window\r\nDo not forget to Save Context when you decide to stop" +
    " the chat with the bot\r\nOtherwise all new bot learnings will be lost\r\n";
            this.botConsole.WordWrap = false;
            this.botConsole.TextChanged += new System.EventHandler(this.botConsole_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.textBoxUserid);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.ucConfigurationButton1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 248);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(745, 141);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "UserID:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.botInput);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(745, 30);
            this.panel2.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(649, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 30);
            this.button1.TabIndex = 3;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // botInput
            // 
            this.botInput.AcceptsReturn = true;
            this.botInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.botInput.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botInput.Location = new System.Drawing.Point(0, 0);
            this.botInput.Name = "botInput";
            this.botInput.Size = new System.Drawing.Size(745, 30);
            this.botInput.TabIndex = 2;
            this.botInput.Visible = false;
            this.botInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.botInput_KeyUp);
            // 
            // textBoxUserid
            // 
            this.textBoxUserid.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUserid.Location = new System.Drawing.Point(71, 52);
            this.textBoxUserid.Name = "textBoxUserid";
            this.textBoxUserid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxUserid.Size = new System.Drawing.Size(100, 26);
            this.textBoxUserid.TabIndex = 8;
            this.textBoxUserid.TextChanged += new System.EventHandler(this.textBoxUserid_TextChanged);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(642, 96);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(91, 33);
            this.button5.TabIndex = 7;
            this.button5.Text = "Clear Console";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(190, 45);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 33);
            this.button3.TabIndex = 5;
            this.button3.Text = "Start Bot";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(309, 45);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 33);
            this.button2.TabIndex = 4;
            this.button2.Text = "Save Context";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ucConfigurationButton1
            // 
            this.ucConfigurationButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ucConfigurationButton1.Image = ((System.Drawing.Image)(resources.GetObject("ucConfigurationButton1.Image")));
            this.ucConfigurationButton1.Location = new System.Drawing.Point(15, 96);
            this.ucConfigurationButton1.Name = "ucConfigurationButton1";
            this.ucConfigurationButton1.Size = new System.Drawing.Size(40, 32);
            this.ucConfigurationButton1.TabIndex = 2;
            this.ucConfigurationButton1.UseVisualStyleBackColor = true;
            this.ucConfigurationButton1.Click += new System.EventHandler(this.ucConfigurationButton1_Click);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(745, 389);
            this.Controls.Add(this.botConsole);
            this.Controls.Add(this.panel1);
            this.Name = "Mainform";
            this.Text = "Mainform";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox botConsole;
        private System.Windows.Forms.Panel panel1;
        private EZ_Builder.UCForms.UC.UCConfigurationButton ucConfigurationButton1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox botInput;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBoxUserid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
    }
}