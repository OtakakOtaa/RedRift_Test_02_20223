using System;
using System.Collections.Generic;
using CodeBase.Hand.Model;
using DG.Tweening;
using NSTools.Core;
using UnityEngine;

namespace CodeBase.Hand.View
{
    public class CardsAnimationPlayer
    {
        private readonly CardsAnimationSettings _settings;

        private readonly CardsPermutationAnimationCash _permutationAnimationCash = new();
        private readonly Dictionary <Transform,ReturnAnimationCash> _returnAnimationCash = new();
        
        public CardsAnimationPlayer(CardsAnimationSettings settings)
        {
            _settings = settings;
        }
        
        public void PlayCardsPermutationAnimation(Transform[] cards, CardsPositionCalculator.CardPosition[] newPosition)
        {
            _permutationAnimationCash.TargetCards = cards;
            _permutationAnimationCash.StartTransformCardsForAnimation = cards;
            _permutationAnimationCash.NewCardsPosition = newPosition;
            DOTween.To(CardsPermutationAnimation, 0f, 1f, _settings.PermutationSpeed);
        }

        public void PlayStartCatchAnimation(Transform card)
        {
            CreateCashForReturn(card);
            card.transform.DORotate(Quaternion.identity.eulerAngles, _settings.RotationSpeed);
        }

        public void PlayCardReturnAnimation(Transform card)
        {
            CheckCardAlreadyReturning(card);
            ReturnToHandAnimation(_returnAnimationCash[card]);
        }
        
        private void CardsPermutationAnimation(float delta)
        {
            var eDelta = EZ.BackIn(delta);
            for (var i = 0; i < _permutationAnimationCash.TargetCards.Length; i++)
            {
                _permutationAnimationCash.TargetCards[i].transform.position = Vector3.Lerp(
                    _permutationAnimationCash.StartTransformCardsForAnimation[i].transform.position,
                    _permutationAnimationCash.NewCardsPosition[i].Position,
                    eDelta);

                _permutationAnimationCash.TargetCards[i].transform.rotation = Quaternion.Lerp(
                    _permutationAnimationCash.StartTransformCardsForAnimation[i].transform.rotation,
                    _permutationAnimationCash.NewCardsPosition[i].Rotation,
                    eDelta);
            }
        }
        
        private void ReturnToHandAnimation(ReturnAnimationCash returnAnimationCash)
        { 
            returnAnimationCash.tweener?.Kill();
            returnAnimationCash.tweener = DOTween.To(Animation, 0f, 1f, _settings.ReturnCardSpeed);
            
            void Animation(float delta)
            {
                var eDelta = EZ.QuadOut(delta);
                returnAnimationCash.ReturnCard.position = Vector3.Lerp(
                    returnAnimationCash.CapturedPositionCardBeforeAnimation,
                    returnAnimationCash.OriginCapturedCardPosition,
                    eDelta);
                returnAnimationCash.ReturnCard.rotation = Quaternion.Lerp(
                    Quaternion.identity,
                    returnAnimationCash.OriginCapturedCardRotation,
                    eDelta);

                bool isLastLoop = Math.Abs(delta - 1) == 0;
                if (isLastLoop)
                    _returnAnimationCash.Remove(returnAnimationCash.ReturnCard);
            }
        }

        private void CheckCardAlreadyReturning(Transform card)
        {
            _returnAnimationCash.TryGetValue(card, out ReturnAnimationCash value);
            if (value is null)
            {
                var position = card.position;
                _returnAnimationCash[card] = new ReturnAnimationCash()
                {
                    ReturnCard = card,
                    CapturedPositionCardBeforeAnimation = position,
                    OriginCapturedCardRotation = card.rotation,
                    OriginCapturedCardPosition = position,
                };
            }
            else
                value.CapturedPositionCardBeforeAnimation = card.position;
        }

        private void CreateCashForReturn(Transform card)
        {
            _returnAnimationCash.TryGetValue(card, out ReturnAnimationCash value);
            if (value is not null)
            {
                value.tweener?.Kill();
                return;
            }
            var position = card.position;
            _returnAnimationCash[card] = new ReturnAnimationCash()
            {
                ReturnCard = card,
                OriginCapturedCardRotation = card.rotation,
                OriginCapturedCardPosition = position,
            };
        }
        
        private class ReturnAnimationCash
        {
            public Vector3 OriginCapturedCardPosition;
            public Quaternion OriginCapturedCardRotation;
            public Vector3 CapturedPositionCardBeforeAnimation;
            public Transform ReturnCard;

            public Tweener tweener;
        }

        private class CardsPermutationAnimationCash
        {
            public Transform[] TargetCards;
            public Transform[] StartTransformCardsForAnimation;
            public CardsPositionCalculator.CardPosition[] NewCardsPosition;

        }

        [Serializable] public class CardsAnimationSettings
        {
            [SerializeField, Range(0, 2)] private float _permutationAnimationSpeed;
            [SerializeField, Range(0, 0.5f)] private float _rotationSpeed;
            [SerializeField, Range(0, 3f)] private float _returnCardAnimationSpeed;
            
            public float PermutationSpeed => _permutationAnimationSpeed;
            public float RotationSpeed => _rotationSpeed;
            public float ReturnCardSpeed => _returnCardAnimationSpeed;
        }
    }
}