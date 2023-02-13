using System;
using System.Linq;
using CodeBase.Hand.Model;
using UnityEngine;
using Random = System.Random;


namespace CodeBase.CardsButton
{
    public class CardsButtonView : MonoBehaviour
    {
        private CardsHand _cardHand;
        private Configuration _configuration;
        
        private int _cardIndex;
        
        public void InjectDependencies(CardsHand cardsHand, Configuration configuration)
        {
            _cardHand = cardsHand;
            _configuration = configuration;

            _cardHand.CardDeleted += RevertCounter;
        }
        
        public void OnClick()
        {
            if(_cardHand is null) return;
            
            ChangeableCardParam selectedParam = SelectCardParam();
            int cardIndex = GetNextCardIndex();
            int newValue = SelectNewParamValue(); 
            
            CheckForRepeat(ref newValue, selectedParam, cardIndex);
            
            switch(selectedParam)
            {
                case ChangeableCardParam.Heath:
                    _cardHand.ChangeCardHealth(newValue,cardIndex);
                    break;
                case ChangeableCardParam.Manacost:
                    _cardHand.ChangeCardManaCost(newValue,cardIndex);
                    break;
                case ChangeableCardParam.Attack:
                    _cardHand.ChangeCardAttack(newValue,cardIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ChangeableCardParam SelectCardParam()
            => (ChangeableCardParam)new Random().Next(0, 3);

        private int SelectNewParamValue()
            => _configuration.ChangedCardParam;

        private int GetNextCardIndex()
        {
            if (_cardIndex < _cardHand.CardsCount) return _cardIndex++;
            _cardIndex = 1;
            return 0;
        }

        private void CheckForRepeat(ref int newValue, ChangeableCardParam selectedParam, int cardIndex)
        {
            while (true)
            {
                bool isRepeated = selectedParam switch
                {
                    ChangeableCardParam.Heath => newValue == _cardHand.Cards.ToArray()[cardIndex].Health,
                    ChangeableCardParam.Manacost => newValue == _cardHand.Cards.ToArray()[cardIndex].ManaCost,
                    ChangeableCardParam.Attack => newValue == _cardHand.Cards.ToArray()[cardIndex].Attack,
                    _ => throw new ArgumentOutOfRangeException(nameof(selectedParam), selectedParam, null)
                };
                if(!isRepeated) return;
                newValue = SelectNewParamValue();
            }
        }

        private void RevertCounter(int index)
            => _cardIndex--;

        private enum ChangeableCardParam { Heath, Manacost, Attack, }
    }
}