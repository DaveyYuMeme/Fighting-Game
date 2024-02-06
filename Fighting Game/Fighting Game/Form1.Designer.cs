namespace Fighting_Game
{
    partial class Form1
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
            this.Panel = new System.Windows.Forms.Panel();
            this.Frame = new System.Windows.Forms.Timer(this.components);
            this.chombo = new System.Windows.Forms.Label();
            this.p2chombo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.Location = new System.Drawing.Point(0, 0);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(800, 600);
            this.Panel.TabIndex = 0;
            this.Panel.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel_Paint);
            // 
            // Frame
            // 
            this.Frame.Enabled = true;
            this.Frame.Interval = 15;
            this.Frame.Tick += new System.EventHandler(this.FrameTick);
            // 
            // chombo
            // 
            this.chombo.AutoSize = true;
            this.chombo.Dock = System.Windows.Forms.DockStyle.Left;
            this.chombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chombo.Location = new System.Drawing.Point(0, 0);
            this.chombo.Name = "chombo";
            this.chombo.Size = new System.Drawing.Size(92, 24);
            this.chombo.TabIndex = 0;
            this.chombo.Text = "Combo: 0";
            // 
            // p2chombo
            // 
            this.p2chombo.AutoSize = true;
            this.p2chombo.Dock = System.Windows.Forms.DockStyle.Right;
            this.p2chombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p2chombo.Location = new System.Drawing.Point(708, 0);
            this.p2chombo.Name = "p2chombo";
            this.p2chombo.Size = new System.Drawing.Size(92, 24);
            this.p2chombo.TabIndex = 1;
            this.p2chombo.Text = "Combo: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.p2chombo);
            this.Controls.Add(this.chombo);
            this.Controls.Add(this.Panel);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Fighter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.Timer Frame;
        private System.Windows.Forms.Label chombo;
        private System.Windows.Forms.Label p2chombo;
    }
}

