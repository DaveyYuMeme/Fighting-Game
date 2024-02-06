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
        public int hitstop = 0, combo = 0, juggle = 0, throwinvuln = 0, knockdown = 0;
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
                    if (animation.animFrame.xV * side < 0)
                    {
                        if (animation.animFrame.xV * side < xV) { xV = animation.animFrame.xV * side; }
                    }
                    else if (animation.animFrame.xV * side > 0)
                    {
                        if (animation.animFrame.xV * side > xV) { xV = animation.animFrame.xV * side; }
                    }
                }
                // Velocity
                if (y > 0) { yV -= 1f; }
                x += xV; y += yV;
                if (y <= 0) { y = 0; if (yV < 0) { yV = 0; animation = new Animation(); } }
                if (y == 0) { xV *= 0.85f; }
                // Knockdowns
                if (!animation.active || (animation.animFrame.flag != "Hitstun" & animation.animFrame.flag != "Blockstun"))
                {
                    if (throwinvuln > 0) { throwinvuln -= 1; }
                }
            }
            actionable = !animation.active;
            if (actionable) {
                if (x < opponent.x) { side = 1; } else { side = -1; }
                combo = 0; juggle = 0;
            }
            if (Math.Abs(x - opponent.x) < 45 & Math.Abs(y - opponent.y) < 140)
            {
                if (x < opponent.x) { x = opponent.x - 45; } else { x = opponent.x + 45; }
            }
            x = Math.Min(Math.Max(x, -370), 370);
        }

        public void Actions(Inputs input)
        {
            di = input.di;
            if (hitstop <= 0)
            {
                // Attacks
                if (input.D & actionable & y == 0)
                {
                    if (Math.Abs(x - opponent.x) < 70 & opponent.y == 0 & opponent.throwinvuln < 1)
                    {
                        // Move opponent
                        if (di == 4) { side *= -1; }
                        opponent.x = x + (70 * side);
                        opponent.xV = 0; opponent.yV = 0;
                        opponent.animation = new Animation(); opponent.animFrame = 1;
                        opponent.animation.frames.Add(new AnimationFrame(40, "Kick1"));
                        opponent.animation.Step(1); opponent.actionable = false;
                        // Reset own anim
                        xV = 0; yV = 0;
                        animFrame = 1;
                        animation = new Animation();
                        AnimationFrame animf;
                        animation.cancelPriority = 5;
                        // Grab anim
                        animf = new AnimationFrame(3, "Jab1");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(18, "Jab2");
                        animation.frames.Add(animf);
                        // Me when I GET you
                        int holdf = 18;
                        animf = new AnimationFrame(holdf + 2, "Elbow1");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 4, "Elbow2");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 6, "Elbow3");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 8, "Elbow4");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 10, "Elbow5");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 18, "Elbow6");
                        animf.flag = "Throw";
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(holdf + 22, "Elbow4");
                        animation.frames.Add(animf);

                        animation.Step(1);
                    }
                    else
                    {
                        animFrame = 1;
                        animation = new Animation();
                        AnimationFrame animf;
                        animf = new AnimationFrame(2, "Jab1");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(6, "Jab2");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(21, "Jab3");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(23, "Jab4");
                        animation.frames.Add(animf);
                        animf = new AnimationFrame(27, "Jab5");
                        animation.frames.Add(animf);
                        animation.Step(1);
                    }
                }
                else if ((input.C || input.AB & input.di == 6) & (actionable || (animation.hit > 0 && animation.cancelPriority < 3)))
                {
                    animFrame = 1;
                    animation = new Animation();
                    AnimationFrame animf;
                    // custom animation
                    animation.cancelPriority = 3;

                    int[] frameDataElbow = { 3, 5, 7, 9, 11, 16, 23, 29 };
                    int aniNumElbow = 1;
                    for (int i = 0; i < frameDataElbow.Length; i++)
                    {
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

                    int[] frameDataKick = { 2, 4, 6, 9, 14, 17, 19, 21 };
                    int aniNumKick = 1;
                    for (int i = 0; i <= 4; i++)
                    {
                        animf = new AnimationFrame(frameDataKick[i], "Kick" + aniNumKick);
                        switch (frameDataKick[i])
                        {
                            case 9:
                                animf.Hitboxes.Add(new Hitbox(new Rectangle(80, -10, 85, 40)));
                                animf.Hurtboxes.Add(new Hitbox(new Rectangle(75, -10, 105, 30)));
                                break;
                            case 14:
                                animf.Hurtboxes.Add(new Hitbox(new Rectangle(75, -10, 105, 30)));
                                break;
                        }
                        animation.frames.Add(animf);
                        aniNumKick++;
                    }
                    aniNumKick = 3;
                    for (int i = 5; i <= 7; i++)
                    {
                        animf = new AnimationFrame(frameDataKick[i], "Kick" + aniNumKick);
                        animation.frames.Add(animf);
                        aniNumKick--;
                    }

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
                    if (input.di == 8) { yV = 16; xV = 0; }
                    else if (input.di == 7) { yV = 16; xV = -4 * side; }
                    else if (input.di == 9) { yV = 16; xV = 4 * side; }

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
        public string flag = "None";

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
