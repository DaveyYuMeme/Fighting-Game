using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fighting_Game
{
    public partial class Form1 : Form
    {
        Game Match = new Game();
        public string filepath = Application.StartupPath.Substring(0, Application.StartupPath.IndexOf("Fighting Game") + 13) + "\\Fighting Game";
        public int baseX = 800, baseY = 600;

        public Dictionary<Keys, Keys> Rebinds = new Dictionary<Keys, Keys>();
        public Dictionary<Keys, bool> KeysDown = new Dictionary<Keys, bool>();
        public Dictionary<Keys, int> InputBuffer = new Dictionary<Keys, int>();

        public bool debug = true;

        public Dictionary<string, Image> Sprites = new Dictionary<string, Image>();

        public Form1()
        {
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            Panel, new object[] { true });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Binds
            Rebinds.Add(Keys.A, Keys.Left); Rebinds.Add(Keys.D, Keys.Right); Rebinds.Add(Keys.W, Keys.Up); Rebinds.Add(Keys.S, Keys.Down);
            Rebinds.Add(Keys.J, Keys.Z); Rebinds.Add(Keys.K, Keys.X); Rebinds.Add(Keys.L, Keys.C); Rebinds.Add(Keys.U, Keys.A);
            // Setup input buffer
            KeysDown.Add(Keys.Left, false); KeysDown.Add(Keys.Right, false); KeysDown.Add(Keys.Up, false); KeysDown.Add(Keys.Down, false);
            KeysDown.Add(Keys.Z, false); KeysDown.Add(Keys.X, false); KeysDown.Add(Keys.C, false); KeysDown.Add(Keys.A, false);
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { InputBuffer.Add(kvp.Key, 0); }
            // Window
            Size = new Size(baseX + 16, baseY + 39);
            Panel.Size = new Size(baseX, baseY);
            // Players
            Match.Player1 = new Character(); Match.Player2 = new Character();
            Match.Characters.Add(Match.Player1); Match.Characters.Add(Match.Player2);
            Match.Player1.opponent = Match.Player2; Match.Player2.opponent = Match.Player1;
            Match.Player1.x = -100;
            Match.Player2.x = 100; Match.Player2.side = -1;
            // Load Resources
            Sprites["Idle"] = new Bitmap(filepath + "\\Resources\\Luna\\Idle.png").Clone(new Rectangle(250, 150, 300, 300), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        }

        private Rectangle DrawPos(int x, int y, int width, int height)
        {
            float aspectRatio = (float)baseX / (float)baseY;
            int windowX = Width - 16, windowY = Height - 39;
            int bufferX = 0, bufferY = 0;
            if (windowX > (float)windowY * aspectRatio)
            {
                bufferX = windowX - (int)((float)windowY * aspectRatio);
                bufferX /= 2;
                windowX = (int)((float)windowY * aspectRatio);
            }
            if (windowY > (float)windowX / aspectRatio)
            {
                bufferY = windowY - (int)((float)windowX / aspectRatio);
                bufferY /= 2;
                windowY = (int)((float)windowX / aspectRatio);
            }
            float scaleX = (float)windowX / (float)baseX, scaleY = (float)windowY / (float)baseY;
            width = (int)((float)width * scaleX); height = (int)((float)height * scaleY);
            return new Rectangle((windowX/2) + bufferX + (int)(x * scaleX) - (width/2), (windowY/2) + bufferY - (int)(y * scaleY) - (height/2), width, height);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel.Size = new Size(Width - 18, Height - 39);
            Draw(e.Graphics);
            Panel.Visible = false;
        }

        private void Draw(Graphics g)
        {
            chombo.Text = "Combo: " + Match.Player2.combo;
            g.FillRectangle(Brushes.LightGray, DrawPos(0, 0, baseX, baseY));
            foreach (Character Char in Match.Characters)
            {
                if (Char.animation.active)
                {
                    g.DrawImage(Image.FromFile(filepath + "\\Resources\\Luna\\" + Char.animation.animFrame.sprite + ".png"), DrawPos((int)Char.x, (int)Char.y, 300 * Char.side, 300), 250, 150, 300, 300, GraphicsUnit.Pixel);
                }
                else
                {
                    //g.DrawImage(Image.FromFile(filepath + "\\Resources\\Luna\\Idle.png"), DrawPos((int)Char.x, (int)Char.y, 300 * Char.side, 300), 250, 150, 300, 300, GraphicsUnit.Pixel);
                    g.DrawImage(Sprites["Idle"], DrawPos((int)Char.x, (int)Char.y, 300 * Char.side, 300));
                }
                if (debug)
                {
                    g.DrawRectangle(new Pen(Brushes.Blue), DrawPos((Char.hurtbox.Center.X * Char.side) + (int)Char.x, Char.hurtbox.Center.Y + (int)Char.y, Char.hurtbox.Bounds.Width, Char.hurtbox.Bounds.Height));
                    foreach (Hitbox hitbox in Char.animation.animFrame.Hurtboxes)
                    {
                        g.DrawRectangle(new Pen(Brushes.Blue), DrawPos((hitbox.Center.X * Char.side) + (int)Char.x, hitbox.Center.Y + (int)Char.y, hitbox.Bounds.Width, hitbox.Bounds.Height));
                    }
                    foreach (Hitbox hitbox in Char.animation.animFrame.Hitboxes)
                    {
                        g.DrawRectangle(new Pen(Brushes.Red), DrawPos((hitbox.Center.X * Char.side) + (int)Char.x, hitbox.Center.Y + (int)Char.y, hitbox.Bounds.Width, hitbox.Bounds.Height));
                    }
                }
            }
            //g.DrawImage(Image.FromFile(filepath + "\\Resources\\Luna\\Idle.png"), new Rectangle(0, 0, 300, 300), 250, 150, 300, 300, GraphicsUnit.Pixel);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Rebinds.TryGetValue(e.KeyCode, out Keys bind))
            {
                KeysDown[bind] = true;
                InputBuffer[bind] = 4;
            }
            else
            {
                KeysDown[e.KeyCode] = true;
                InputBuffer[e.KeyCode] = 4;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (Rebinds.TryGetValue(e.KeyCode, out Keys bind))
            {
                KeysDown[bind] = false;
            }
            else
            {
                KeysDown[e.KeyCode] = false;
            }
        }

        Random rand = new Random();

        private void FrameTick(object sender, EventArgs e)
        {
            // Interpret Inputs
            Inputs NewInputLine = new Inputs();
            NewInputLine.PlayerInput(KeysDown, InputBuffer);
            Inputs P2InputLine = new Inputs();
            P2InputLine.di = 1;
            if (rand.Next(0, 3) == 0) { P2InputLine.di = 5; }
            if (rand.Next(0, 60) == 1) { P2InputLine.A = true; }
            if (rand.Next(0, 60) == 1) { P2InputLine.B = true; }
            if (rand.Next(0, 60) == 1) { P2InputLine.C = true; }
            Match.Step(NewInputLine, P2InputLine);
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown)
            {
                if (InputBuffer[kvp.Key] > 0) { InputBuffer[kvp.Key] -= 1; }
            }
            // Resolve
            // Redraw
            //Panel.Invalidate();
            //Panel.Update();
            Invalidate();
        }
    }
}
