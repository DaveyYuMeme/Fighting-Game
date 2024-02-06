using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fighting_Game
{
    class Game
    {
        public Character Player1, Player2;
        public List<Character> Characters = new List<Character>();
        public int hitstop = 0, spellflash = 0;
        public string state = "Fight";
        public int timer = 99, kotimer = 20;

        public void Hitboxes(Character Attacker, Character Defender)
        {
            if (Attacker.animation.hit != Attacker.animation.animFrame.hit)
            {
                // Throw Flags
                if (Attacker.animation.animFrame.flag == "Throw")
                {
                    Attacker.animation.hit = 1;
                    Defender.hp -= 40;
                    // launch
                    Defender.yV = 12f; Defender.juggle = 4; Defender.throwinvuln = 5;
                    Defender.animation = new Animation();
                    Defender.animation.frames.Add(new AnimationFrame(999, "Kick1"));
                    Defender.animation.frames[0].flag = "Hitstun";
                    Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                }
                // Hitboxes
                foreach (Hitbox hitbox in Attacker.animation.animFrame.Hitboxes)
                {
                    if (hitbox.HitCharacter(new Point((int)Attacker.x, (int)Attacker.y), Defender, Attacker.side))
                    {
                        // Hitstop and set Hit Flag
                        if (Attacker.x < Defender.x) { Defender.xV = 5; } else { Defender.xV = -5; }
                        Attacker.hitstop = 8; Defender.hitstop = 8;
                        Attacker.animation.hit = Attacker.animation.animFrame.hit;
                        // Stagger animation
                        if (Defender.actionable & (Defender.di == 4 || Defender.di == 1 || Defender.di == 7))
                        { // Block
                            if (Attacker.y == 0) {if (Attacker.x < Defender.x) { Attacker.xV = -5; } else { Attacker.xV = 5; }}
                            Defender.hp -= 4; // chip
                            Defender.animation = new Animation();
                            if (Attacker.animation.cancelPriority == 1)
                            {
                                Defender.animation.frames.Add(new AnimationFrame(5, "Elbow2"));
                            }
                            else
                            {
                                Defender.animation.frames.Add(new AnimationFrame(8, "Elbow2"));
                            }
                            Defender.animation.frames[0].flag = "Blockstun";
                            Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                            Defender.throwinvuln = 5;
                        }
                        else
                        { // Hit
                            if (Defender.y > 0)
                            { // Juggle
                                if (Attacker.animation.cancelPriority == 1) { Defender.hp -= 10; } else { Defender.hp -= 20; }
                                if (Attacker.x < Defender.x) { Defender.xV = 3; } else { Defender.xV = -3; }
                                Defender.yV = 14f * (14f / (14f + (4 * Defender.juggle)));

                                Defender.animation = new Animation();
                                Defender.animation.frames.Add(new AnimationFrame(999, "Kick1"));
                                Defender.animation.frames[0].flag = "Hitstun";
                                Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                                Defender.combo += 1; Defender.juggle += 1;
                            }
                            else
                            { // Stagger
                                Defender.animation = new Animation();
                                if (Attacker.animation.cancelPriority == 1)
                                {
                                    Defender.hp -= 20;
                                    Defender.animation.frames.Add(new AnimationFrame(9, "Kick1"));
                                }
                                else
                                {
                                    Defender.hp -= 30;
                                    Defender.animation.frames.Add(new AnimationFrame(14, "Kick1"));
                                }
                                Defender.animation.frames[0].flag = "Hitstun";
                                Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                                Defender.combo += 1; Defender.throwinvuln = 5;
                            }
                        }
                    }
                }
            }
        }

        public void Step(Inputs Player1Input, Inputs Player2Input)
        {
            if (hitstop > 0 | spellflash > 0)
            {
                if (hitstop > 0) { hitstop -= 1; }
                if (spellflash > 0) { spellflash -= 1; }
            }
            else
            {
                // Actions
                Player1.Actions(Player1Input); Player2.Actions(Player2Input);
                // Resolve
                foreach (Character Char in Characters)
                {
                    Char.Step();
                }
                Hitboxes(Player1, Player2);
                Hitboxes(Player2, Player1);
            }

            if (Player1.hp <= 0 | Player2.hp <= 0)
            {
                state = "KO";
                if (Player1.animation.active == false & Player2.animation.active == false) { kotimer -= 1; }
                if (kotimer < 0)
                {
                    kotimer = 20; state = "Fight";
                    Player1.hp = 300; Player2.hp = 300;
                    Player1.x = -100; Player2.x = 100;
                }
            }
        }
    }
}