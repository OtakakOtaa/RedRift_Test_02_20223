using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.DragAndDrop;
using CodeBase.Hand.Model;
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Hand.View
{
    public class CardHandView
    {
        private readonly Settings _settings;

        private readonly IGameObjectCreator _creator;
        private readonly IDestroyer _destroyer;

        private readonly CardsHand _cardsHand;
        private readonly CardsPositionCalculator _cardsPositionCalculator;
        private readonly LayerCardSorter _layerCardSorter;
        private readonly CardHolder _cardHolder;
        private readonly CardsAnimationPlayer _cardsAnimationPlayer;

        private readonly List<CardView> _cards = new();

        private bool _isFirstCardsPut = true;
        private CardView _capturedCard;

        public CardHandView(CardsHand cardsHand, IEnumerable<Sprite> images, 
            IDestroyer destroyer, IGameObjectCreator creator, Transform cardHolderPosition, Settings settings)
        {
            _settings = settings;
            _destroyer = destroyer;
            _cardsHand = cardsHand;
            _creator = creator;
            
            _cardsPositionCalculator = new CardsPositionCalculator(settings.PositionCalculatorSettings);
            _cardsAnimationPlayer = new CardsAnimationPlayer(settings.CardsAnimationSettings);
            _layerCardSorter = new LayerCardSorter();
            _cardHolder = new CardHolder(cardHolderPosition);

            cardsHand.CardChanged += UpdateCard;
            cardsHand.CardDeleted += DeleteCard;
            
            CreateCardsView(images);
            SubscribeToCardViews();
        }

        public void PutCards()
        {
            var putPoints = _cardsPositionCalculator.GetPositionPoints(_cards.Count).ToArray();
            if (_isFirstCardsPut)
            {
                for (var i = 0; i < _cards.Count; i++) 
                {
                    _cards[i].transform.position = putPoints[i].Position;
                    _cards[i].transform.rotation = putPoints[i].Rotation;
                }
                
                SortCardsByLayerOrder();
                _isFirstCardsPut = false;
                return;
            }
            
            _cardsAnimationPlayer.PlayCardsPermutationAnimation(
                cards: _cards.Select(_ => _.transform).ToArray(),
                newPosition: putPoints
            );
            
            SortCardsByLayerOrder(); 
        }

        private void UpdateCard(int position, Card card)
        {
            _cards[position].UpdateParam(card);
        }

        private void DeleteCard(int position)
        {
            UnSubscribeFromCardView(_cards[position]);
            _destroyer.DestroyObject(_cards[position].gameObject);
            _cards.RemoveAt(position);
            PutCards();
        }

        private void CreateCardsView(IEnumerable<Sprite> images)
        {
            for (int i = 0; i < _cardsHand.Cards.Count(); i++)
            {
                var cardView = _creator.CreateByPrefab(_settings.CardPrefab).GetComponent<CardView>();
                cardView.UpdateParam(_cardsHand.Cards.ToArray()[i]);
                cardView.SetImage(images.ToArray()[i]);
                _cards.Add(cardView);
            }
        }

        private void SortCardsByLayerOrder()
            => _layerCardSorter.SortCards(_cards);

        
        private void OnStartCaptured(Transform capturedCard)
        {
            _capturedCard = capturedCard.GetComponent<CardView>();
            _cardsAnimationPlayer.PlayStartCatchAnimation(_capturedCard.transform);
        }
        
        private void OnEndCaptured()
        {
            if (_cardHolder.TryCanHoldCard(_capturedCard))
            {
                PutCards();
                return;
            }
            _cardsAnimationPlayer.PlayCardReturnAnimation(_capturedCard.transform);
        }

        private void SubscribeToCardViews()
        {
            foreach (var card in _cards)
            {
                card.InitializeEnded += () =>
                {
                    card.GrippeableObject.StartCaptured += OnStartCaptured;
                    card.GrippeableObject.StopCaptured += OnEndCaptured;
                };
            }
        }

        private void UnSubscribeFromCardView(CardView cardView)
        {
            cardView.GrippeableObject.StartCaptured -= OnStartCaptured;
            cardView.GrippeableObject.StopCaptured -= OnEndCaptured;
        }

        [Serializable] public class Settings
        {
            [SerializeField] private CardsPositionCalculator.Settings _positionCalculatorSettings;
            [SerializeField] private GameObject _cardPrefab;
            [SerializeField] private CardsAnimationPlayer.CardsAnimationSettings _cardsAnimationSettings;
            
            public GameObject CardPrefab => _cardPrefab;
            public CardsPositionCalculator.Settings PositionCalculatorSettings => _positionCalculatorSettings;
            public CardsAnimationPlayer.CardsAnimationSettings CardsAnimationSettings => _cardsAnimationSettings;
        }
    }
}