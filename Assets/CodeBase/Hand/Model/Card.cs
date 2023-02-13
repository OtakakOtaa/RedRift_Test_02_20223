using System;

namespace CodeBase.Hand.Model
{
    public class Card
    {
        public int Health { get; private set; }

        public int ManaCost { get; private set; }

        public int Attack { get; private set; }

        
        public Card(int health, int manaCost, int attack)
        {
            if (IsValidValue(health) && IsValidValue(manaCost) && IsValidValue(attack) is false)
                throw new Exception();
            
            Health = health;
            ManaCost = manaCost;
            Attack = attack;
        }

        public bool TrySetAttack(int attack)
        {
            if (!IsValidValue(attack)) return false;
            Attack = attack;
            return true;
        }
        
        public bool TrySetManaCost(int manaCost)
        {
            if (!IsValidValue(manaCost)) return false;
            ManaCost = manaCost;
            return true;
        }
        
        public bool TrySetHealth(int health)
        {
            if (!IsValidValue(health)) return false;
            Health = health;
            return true;
        }
        
        private bool IsValidValue(int value) => value > 0;
    }
}