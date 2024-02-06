using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fighting_Game
{
    class Character
    {
        public float x = 0, y = 0;
        public float xV = 0, yV = 0;
        public int hp = 300;
        public int crouch = 0, side = 1, di = 5;
        public int hitstop = 0, combo = 0, throwinvuln = 0, knockdown = 0;
        public bool actionable = true;

        public Character opponent;

        public bool animActive = false;
        public int animFrame = 0;
        public Animation animation = new Animation();

        public Hitbox hurtbox = new Hitbox(new Rectangle(-4, -10, 60, 170));

        public void Step()
        {
            if (hitstop > 0)
            {
                hitstop -= 1;
                animation.Step(animFrame);
            }
            else
            {
                // Animation
                if (animation.frames.Count() > 0)
                {
                    animFrame += 1;
                    animation.Step(animFrame);
                    xV += animation.animFrame.xV * side;
                }
                // Velocity
                if (y > 0) { yV -= 1f; }
                x += xV; y += yV;
                if (y <= 0) { y = 0; if (yV < 0) { yV = 0; animation = new Animation(); } }
                if (y == 0) { xV *= 0.8f; }
            }
            actionable = !animation.active;
            if (actionable) { combo = 0; }
        }

        public void Actions(Inputs input)
        {
            di = input.di;
            if (hitstop <= 0)
            {
                // Attacks
                if ((input.C || input.AB & input.di == 6) & (actionable || (animation.hit > 0 && animation.cancelPriority < 3)))
                {
                    animFrame = 1;
                    animation = new Animation();
                    AnimationFrame animf;
                    // custom animation
                    animation.cancelPriority = 3;

                    /*animf = new AnimationFrame(3, "Elbow1");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(5, "Elbow2");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(7, "Elbow3");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(9, "Elbow4");
                    animf.xV = 2;
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(11, "Elbow5");
                    animf.xV = 3;
                    animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(16, "Elbow6");
                    animf.Hitboxes.Add(new Hitbox(new Rectangle(70, 20, 50, 50)));
                    animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(23, "Elbow6");
                    animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(29, "Elbow4");
                    animation.frames.Add(animf);*/

                    int[] frameDataElbow = { 3, 5, 7, 9, 11, 16, 23, 29 };
                    int aniNumElbow = 1;
                    for(int i = 0; i<frameDataElbow.Length; i++){
                        animf = new AnimationFrame(frameDataElbow[i], "Elbow" + aniNumElbow);
                        switch (frameDataElbow[i])
                        {
                            case 9: 
                                animf.xV = 2; 
                                break;
                            case 11:
                                animf.xV = 3;
                                animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                                break;
                            case 16:
                                animf.Hitboxes.Add(new Hitbox(new Rectangle(70, 20, 50, 50)));
                                animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                                break;
                            case 23:
                                animf.Hurtboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 70)));
                                break;
                        }
                        animation.frames.Add(animf);
                        aniNumElbow++;
                    }

                    animation.Step(1);
                }
                else if ((input.B) & (actionable || (animation.hit > 0 & animation.cancelPriority < 2)))
                {
                    animFrame = 1;
                    animation = new Animation();
                    AnimationFrame animf;
                    // custom animation
                    animation.cancelPriority = 2;

                    animf = new AnimationFrame();
                    animf.frame = 2; animf.sprite = "Kick1";
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 4; animf.sprite = "Kick2";
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 6; animf.sprite = "Kick3";
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 9; animf.sprite = "Kick4";
                    animf.Hitboxes.Add(new Hitbox(new Rectangle(80, -10, 85, 40)));
                    animf.Hurtboxes.Add(new Hitbox(new Rectangle(75, -10, 105, 30)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 14; animf.sprite = "Kick5";
                    animf.Hurtboxes.Add(new Hitbox(new Rectangle(75, -10, 105, 30)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 17; animf.sprite = "Kick3";
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 19; animf.sprite = "Kick2";
                    animation.frames.Add(animf);

                    animf = new AnimationFrame();
                    animf.frame = 21; animf.sprite = "Kick1";
                    animation.frames.Add(animf);

                    animation.Step(1);
                }
                else if (input.A & actionable)
                {
                    animFrame = 1;
                    animation = new Animation();
                    AnimationFrame animf;
                    // custom animation
                    animation.cancelPriority = 1;

                    animf = new AnimationFrame(3, "Jab1");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(5, "Jab2");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(8, "Jab3");
                    animf.Hitboxes.Add(new Hitbox(new Rectangle(50, 20, 50, 30)));
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(13, "Jab4");
                    animation.frames.Add(animf);

                    animf = new AnimationFrame(15, "Jab5");
                    animation.frames.Add(animf);

                    animation.Step(1);
                }
            }
            if (actionable & hitstop <= 0)
            {
                int walkspeed = 4;
                if (y == 0)
                {
                    // Jumping
                    if (input.di == 8) { yV = 16; }
                    else if (input.di == 7) { yV = 16; xV = -4; }
                    else if (input.di == 9) { yV = 16; xV = 4; }

                    // Walking
                    if (input.di == 6)
                    {
                        if (side == 1) { x += walkspeed; }
                        else { x -= walkspeed; }
                    }
                    else if (input.di == 4)
                    {
                        if (side == 1) { x -= walkspeed; }
                        else { x += walkspeed; }
                    }
                }
            }
        }
    }

    class Hitbox
    {
        public Rectangle Bounds;
        public Rectangle Center;

        public Hitbox(Rectangle rect = new Rectangle())
        {
            Center = rect;
            rect.Offset(new Point(rect.Width / -2, rect.Height / -2));
            Bounds = rect;
        }

        public bool HitCharacter(Point Origin, Character Defender, int side = 1)
        {
            bool hit = false;

            Rectangle Hit = new Rectangle(Origin.X + (Bounds.X * side), Origin.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            if (side < 0) { Hit.Offset(new Point(Bounds.Width * -1, 0)); }

            Rectangle Hurt = new Rectangle((int)Defender.x + (Defender.hurtbox.Bounds.X * Defender.side), (int)Defender.y + Defender.hurtbox.Bounds.Y, Defender.hurtbox.Bounds.Width, Defender.hurtbox.Bounds.Height);
            if (Defender.side < 0) { Hurt.Offset(new Point(Defender.hurtbox.Bounds.Width * -1, 0)); }
            if (Hurt.IntersectsWith(Hit)) { hit = true; }
            foreach (Hitbox hurtbox in Defender.animation.animFrame.Hurtboxes)
            {
                Hurt = new Rectangle((int)Defender.x + (hurtbox.Bounds.X * Defender.side), (int)Defender.y + hurtbox.Bounds.Y, hurtbox.Bounds.Width, hurtbox.Bounds.Height);
                if (Defender.side < 0) { Hurt.Offset(new Point(hurtbox.Bounds.Width * -1, 0)); }
                if (Hurt.IntersectsWith(Hit)) { hit = true; }
            }

            return hit;
        }
    }

    class Animation
    {
        public AnimationFrame animFrame = new AnimationFrame();
        public List<AnimationFrame> frames = new List<AnimationFrame>();
        public bool active = false; public int hit = 0;
        public int cancelPriority = 0;

        public void Step(int frame)
        {
            active = false;
            int latestFrame = 0;
            foreach (AnimationFrame f in frames)
            {
                if (frame <= f.frame & (f.frame < latestFrame | latestFrame == 0))
                {
                    latestFrame = f.frame; animFrame = f; active = true;
                }
            }
            if (!active) { hit = 0; }
        }
    }
    class AnimationFrame
    {
        public string sprite = "Idle";
        public int frame = 0, hit = 1;
        public float xV = 0, xY = 0;
        public List<Hitbox> Hitboxes = new List<Hitbox>();
        public List<Hitbox> Hurtboxes = new List<Hitbox>();

        public AnimationFrame(int _frame = 0, string _sprite = "Idle")
        {
            frame = _frame; sprite = _sprite;
        }
    }

    class Inputs
    {
        public int di = 5;
        public bool A = false, B = false, C = false, D = false;
        public bool AB = false, BC = false;

        public void PlayerInput(Dictionary<Keys, bool> KeysDown, Dictionary<Keys, int> InputBuffer)
        {
            if (KeysDown[Keys.Left]) { di = 4; }
            if (KeysDown[Keys.Right]) { if (di == 4) { di = 5; } else { di = 6; } }

            if (KeysDown[Keys.Up]) { di += 3; }
            else if (KeysDown[Keys.Down]) { di -= 3; }

            A = (InputBuffer[Keys.Z] > 0);
            B = (InputBuffer[Keys.X] > 0);
            C = (InputBuffer[Keys.C] > 0);
            D = (InputBuffer[Keys.A] > 0);

            AB = (A & B & KeysDown[Keys.Z] & KeysDown[Keys.X]);
            BC = (B & C & KeysDown[Keys.X] & KeysDown[Keys.C]);
        }
    }
}
