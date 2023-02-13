using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace CodeBase
{
    [Serializable]
    public struct Configuration
    {
        [Header("Cards")] [SerializeField]
        private Vector2 _startAmountCardsInHand;

        [SerializeField]
        private Vector2 _startAmountParamRange;

        [SerializeField]
        private Vector2 _changeParamValueRange;

        public int CardsAmountsInHand =>
            new Random().Next
            (
                minValue: (int)_startAmountCardsInHand.x,
                maxValue: (int)_startAmountCardsInHand.y
            );

        public int StartCardParam =>
            new Random().Next
            (
                minValue: (int)_startAmountParamRange.x,
                maxValue: (int)_startAmountParamRange.y
            );

        public int ChangedCardParam =>
            new Random().Next
            (
                minValue: (int)_changeParamValueRange.x,
                maxValue: (int)_changeParamValueRange.y
            );
    }
}