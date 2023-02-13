using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.DragAndDrop;
using CodeBase.Hand.Model;
using CodeBase.Infrastructure;
using DG.Tweening;
using NSTools.Core;
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

        private readonly List<CardView> _cards = new();

        private bool _isFirstCardsPut = true;
        private Transform[] _startTransformCardsForAnimation;
        private CardsPositionCalculator.CardPosition[] _newCardsPosition;

        private Vector3 _originCapturedCardPosition;
        private Quaternion _originCapturedCardRotation;
        private Vector3 _capturedPositionCardBeforeAnimation;
        private CardView _capturedCard;

        public CardHandView(CardsHand cardsHand, IEnumerable<Sprite> images,
            IDestroyer destroyer, IGameObjectCreator creator, Transform cardHolderPosition, Settings settings)
        {
            _settings = settings;
            _destroyer = destroyer;
            _cardsHand = cardsHand;
            _creator = creator;
            _cardsPositionCalculator = new CardsPositionCalculator(settings.PositionCalculatorSettings);
            _layerCardSorter = new LayerCardSorter();
            _cardHolder = new CardHolder(cardHolderPosition);
            
            cardsHand.CardChanged += UpdateCard;
            cardsHand.CardDeleted += DeleteCard;
            CreateCardsView(images);
            
            foreach (var card in _cards)
            {
                card.InitializeEnded += () =>
                {
                    card.GrippeableObject.StartCaptured += () => OnStartCaptured(card);
                    card.GrippeableObject.StopCaptured += OnEndCaptured;
                };
            }
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

            _newCardsPosition = putPoints;
            _startTransformCardsForAnimation = _cards.Select(card => card.transform).ToArray();
            DOTween.To(CardsPermutationAnimation, 0f, 1f, _settings.PermutationSpeed);
            SortCardsByLayerOrder();
        }

        private void UpdateCard(int position, Card card)
        {
            _cards[position].UpdateParam(card);
        }

        private void DeleteCard(int position)
        {
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

        private void CardsPermutationAnimation(float delta)
        {
            var eDelta = EZ.BackIn(delta);
            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i].transform.position = Vector3.Lerp(
                    _startTransformCardsForAnimation[i].transform.position,
                    _newCardsPosition[i].Position,
                    eDelta);

                _cards[i].transform.rotation = Quaternion.Lerp(
                    _startTransformCardsForAnimation[i].transform.rotation,
                    _newCardsPosition[i].Rotation,
                    eDelta);
            }
        }

        private void OnStartCaptured(CardView capturedCard)
        {
            var transform = capturedCard.transform;
            _originCapturedCardPosition = transform.position;
            _originCapturedCardRotation = transform.rotation;

            _capturedCard = capturedCard;
            _capturedCard.transform.DORotate(Quaternion.identity.eulerAngles, _settings.RotationSpeed);
        }

        private void OnEndCaptured()
        {
            if (_cardHolder.TryCanHoldCard(_capturedCard))
            {
                PutCards();
                return;
            }
            
            _capturedPositionCardBeforeAnimation = _capturedCard.transform.position;
            DOTween.To(ReturnToHandAnimation, 0f, 1f, _settings.ReturnCardSpeed);

            void ReturnToHandAnimation(float delta)
            {
                var eDelta = EZ.QuadOut(delta);
                _capturedCard.transform.position = Vector3.Lerp(
                    _capturedPositionCardBeforeAnimation,
                    _originCapturedCardPosition,
                    eDelta);
                _capturedCard.transform.rotation = Quaternion.Lerp(
                    Quaternion.identity,
                    _originCapturedCardRotation,
                    eDelta);
            }
        }


        [Serializable]
        public class Settings
        {
            [SerializeField] private CardsPositionCalculator.Settings _positionCalculatorSettings;
            [SerializeField] private GameObject _cardPrefab;
            [SerializeField, Range(0, 2)] private float _permutationAnimationSpeed;
            [SerializeField, Range(0, 0.5f)] private float _rotationSpeed;
            [SerializeField, Range(0, 3f)] private float _returnCardAnimationSpeed;

            public GameObject CardPrefab => _cardPrefab;
            public CardsPositionCalculator.Settings PositionCalculatorSettings => _positionCalculatorSettings;
            public float PermutationSpeed => _permutationAnimationSpeed;
            public float RotationSpeed => _rotationSpeed;
            public float ReturnCardSpeed => _returnCardAnimationSpeed;
        }
    }
}