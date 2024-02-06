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

        public Dictionary<Keys, Keys> P2Rebinds = new Dictionary<Keys, Keys>();
        public Dictionary<Keys, bool> P2KeysDown = new Dictionary<Keys, bool>();
        public Dictionary<Keys, int> P2InputBuffer = new Dictionary<Keys, int>();

        public bool debug = false, versusmode = false;

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
            // P2 Binds
            P2Rebinds.Add(Keys.NumPad1, Keys.Left); P2Rebinds.Add(Keys.NumPad3, Keys.Right); P2Rebinds.Add(Keys.NumPad5, Keys.Up); P2Rebinds.Add(Keys.NumPad2, Keys.Down);
            P2Rebinds.Add(Keys.Left, Keys.Z); P2Rebinds.Add(Keys.Down, Keys.X); P2Rebinds.Add(Keys.Right, Keys.C); P2Rebinds.Add(Keys.Up, Keys.A);
            // Setup input buffer
            KeysDown.Add(Keys.Left, false); KeysDown.Add(Keys.Right, false); KeysDown.Add(Keys.Up, false); KeysDown.Add(Keys.Down, false);
            KeysDown.Add(Keys.Z, false); KeysDown.Add(Keys.X, false); KeysDown.Add(Keys.C, false); KeysDown.Add(Keys.A, false);
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { InputBuffer.Add(kvp.Key, 0); }
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { P2KeysDown.Add(kvp.Key, true); }
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { P2InputBuffer.Add(kvp.Key, 0); }
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
            p2chombo.Text = "Combo: " + Match.Player1.combo;
            g.FillRectangle(Brushes.LightGray, DrawPos(0, 0, baseX, baseY));
            // Characters
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
            // System
            g.FillRectangle(Brushes.Orange, DrawPos(-400 + (int)(125 * ((float)Match.Player1.hp / 300)), 250, (int)(250 * ((float)Match.Player1.hp / 300)), 25));
            g.DrawRectangle(new Pen(Brushes.OrangeRed), DrawPos(-275, 250, 250, 25));
            g.FillRectangle(Brushes.Orange, DrawPos(400 - (int)(125 * ((float)Match.Player2.hp / 300)), 250, (int)(250 * ((float)Match.Player2.hp / 300)), 25));
            g.DrawRectangle(new Pen(Brushes.OrangeRed), DrawPos(275, 250, 250, 25));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (versusmode & P2Rebinds.TryGetValue(e.KeyCode, out Keys p2bind))
            {
                P2KeysDown[p2bind] = true;
                P2InputBuffer[p2bind] = 4;
            }
            else if (Rebinds.TryGetValue(e.KeyCode, out Keys bind))
            {
                KeysDown[bind] = true;
                InputBuffer[bind] = 4;
            }
            else
            {
                KeysDown[e.KeyCode] = true;
                InputBuffer[e.KeyCode] = 4;
            }
            // debug binds
            if (e.KeyCode == Keys.V) { debug = !debug; }
            if (e.KeyCode == Keys.B & !versusmode)
            { // enable 2 player mode
                versusmode = true;
                Rebinds.Remove(Keys.J); Rebinds.Add(Keys.G, Keys.Z);
                Rebinds.Remove(Keys.K); Rebinds.Add(Keys.H, Keys.X);
                Rebinds.Remove(Keys.L); Rebinds.Add(Keys.J, Keys.C);
                Rebinds.Remove(Keys.U); Rebinds.Add(Keys.T, Keys.A);
                foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { P2KeysDown[kvp.Key] = false; }
                foreach (KeyValuePair<Keys, bool> kvp in KeysDown) { P2InputBuffer[kvp.Key] = 0; }
                foreach (KeyValuePair<Keys, bool> kvp in P2KeysDown) { KeysDown[kvp.Key] = false; }
                foreach (KeyValuePair<Keys, bool> kvp in P2KeysDown) { InputBuffer[kvp.Key] = 0; }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (versusmode & P2Rebinds.TryGetValue(e.KeyCode, out Keys p2bind))
            {
                P2KeysDown[p2bind] = false;
            }
            else if (Rebinds.TryGetValue(e.KeyCode, out Keys bind))
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
            // Game State
            if (Match.state == "KO") { Frame.Interval = 30; } else { Frame.Interval = 15; }
            // Interpret Inputs
            Inputs NewInputLine = new Inputs();
            Inputs P2InputLine = new Inputs();
            if (Match.state == "Fight")
            {
                // P1
                NewInputLine.PlayerInput(KeysDown, InputBuffer);
                if (Match.Player1.x > Match.Player2.x)
                {
                    if (NewInputLine.di == 1 | NewInputLine.di == 4 | NewInputLine.di == 7) { NewInputLine.di += 2; }
                    else if (NewInputLine.di == 3 | NewInputLine.di == 6 | NewInputLine.di == 9) { NewInputLine.di -= 2; }
                }
                // P2
                if (versusmode)
                {
                    P2InputLine.PlayerInput(P2KeysDown, P2InputBuffer);
                    if (Match.Player2.x > Match.Player1.x)
                    {
                        if (P2InputLine.di == 1 | P2InputLine.di == 4 | P2InputLine.di == 7) { P2InputLine.di += 2; }
                        else if (P2InputLine.di == 3 | P2InputLine.di == 6 | P2InputLine.di == 9) { P2InputLine.di -= 2; }
                    }
                }
                else
                {
                    P2InputLine.di = 1;
                    if (rand.Next(0, 3) == 0) { P2InputLine.di = 5; }
                    if (rand.Next(0, 60) == 1) { P2InputLine.di = 9; }
                    if (rand.Next(0, 60) == 1) { P2InputLine.A = true; }
                    if (rand.Next(0, 60) == 1) { P2InputLine.B = true; }
                    if (rand.Next(0, 60) == 1) { P2InputLine.C = true; }
                    if (Match.Player2.animation.active & Match.Player2.animation.hit > 0)
                    {
                        if (Match.Player2.animation.cancelPriority < 2)
                        {
                            if (rand.Next(0, 3) == 0) { P2InputLine.A = true; P2InputLine.B = true; }
                        }
                        else
                        {
                            if (rand.Next(0, 3) == 0) { P2InputLine.A = true; P2InputLine.B = true; P2InputLine.C = true; }
                        }
                    }
                }
            }
            // Resolve
            Match.Step(NewInputLine, P2InputLine);
            foreach (KeyValuePair<Keys, bool> kvp in KeysDown)
            {
                if (InputBuffer[kvp.Key] > 0) { InputBuffer[kvp.Key] -= 1; }
            }
            foreach (KeyValuePair<Keys, bool> kvp in P2KeysDown)
            {
                if (P2InputBuffer[kvp.Key] > 0) { P2InputBuffer[kvp.Key] -= 1; }
            }
            // Redraw
            //Panel.Invalidate();
            //Panel.Update();
            Invalidate();
        }
    }
}
