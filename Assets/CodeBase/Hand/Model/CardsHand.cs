using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeBase.Hand.Model
{
    public class CardsHand
    {
        private readonly List<Card> _cards;

        public event Action<int, Card> CardChanged;
        public event Action<int> CardDeleted;
        
        public CardsHand(IEnumerable<Card> cards)
        {
            _cards = cards.ToList();
        }

        public IEnumerable<Card> Cards => _cards;
        public int CardsCount => _cards.Count;

        public void ChangeCardHealth(int amount, int cardNumber)
        {
            bool isChangeApproved = _cards[cardNumber].TrySetHealth(amount);
            if (isChangeApproved)
            {
                CardChanged?.Invoke(cardNumber, _cards[cardNumber]);
                return;
            }
            
            _cards.RemoveAt(cardNumber);
            CardDeleted?.Invoke(cardNumber);
        }

        public void ChangeCardManaCost(int amount, int cardNumber)
        {
            bool isChangeApproved = _cards[cardNumber].TrySetManaCost(amount);
            if (!isChangeApproved)
            {
                const int manaCostMinimum = 1;
                _cards[cardNumber].TrySetManaCost(manaCostMinimum);
            }
            CardChanged?.Invoke(cardNumber, _cards[cardNumber]);
        }
        
        public void ChangeCardAttack(int amount, int cardNumber)
        {
            bool isChangeApproved = _cards[cardNumber].TrySetAttack(amount);
            if (!isChangeApproved)
            {
                const int damageMinimum = 1;
                _cards[cardNumber].TrySetAttack(damageMinimum);    
            }
            CardChanged?.Invoke(cardNumber, _cards[cardNumber]);
        }
    }
}