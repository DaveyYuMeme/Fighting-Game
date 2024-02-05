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

        public void Hitboxes(Character Attacker, Character Defender)
        {
            if (Attacker.animation.hit != Attacker.animation.animFrame.hit)
            {
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
                        {
                            // Block
                            Defender.animation = new Animation();
                            if (Attacker.animation.cancelPriority == 1)
                            {
                                Defender.animation.frames.Add(new AnimationFrame(5, "Elbow2"));
                            }
                            else
                            {
                                Defender.animation.frames.Add(new AnimationFrame(8, "Elbow2"));
                            }
                            Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                        }
                        else
                        {
                            // Stagger
                            Defender.animation = new Animation();
                            if (Attacker.animation.cancelPriority == 1)
                            {
                                Defender.animation.frames.Add(new AnimationFrame(9, "Kick1"));
                            }
                            else
                            {
                                Defender.animation.frames.Add(new AnimationFrame(14, "Kick1"));
                            }
                            Defender.animFrame = 0; Defender.animation.Step(0); Defender.actionable = false;
                            Defender.combo += 1;
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
        }
    }
}