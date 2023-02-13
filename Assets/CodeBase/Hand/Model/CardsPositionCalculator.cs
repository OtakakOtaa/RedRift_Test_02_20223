using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Hand.Model
{
    public class CardsPositionCalculator
    {
        private readonly Settings _settings;
        
        public CardsPositionCalculator(Settings settings)
        {
            _settings = settings;
        }
        
        public IEnumerable<CardPosition> GetPositionPoints(int cardsCount)
        {
            CardPosition[] result = new CardPosition[cardsCount];
            result = result.Select(cardsCount => cardsCount = new CardPosition()).ToArray();

            bool isCardsCountEven = cardsCount % 2 == 0.0f;

            Vector3 indentForLeftCards = new Vector3(-Math.Abs(_settings.Indent.x),-Math.Abs(_settings.Indent.y)); 
            Vector3 indentForRightCards = new Vector3(Math.Abs(_settings.Indent.x),-Math.Abs(_settings.Indent.y));

            int deltaOffset = _settings.OffsetAngle / cardsCount;
            int currentOffset = 0;

            int lastLeftCard;
            int firstRightCard;

            CardPosition leftPivotPoint;
            CardPosition rightPivotPoint;

            int sideCardsCount = cardsCount / 2;
            
            if (isCardsCountEven)
            {
                lastLeftCard = cardsCount / 2 - 1;
                firstRightCard = cardsCount / 2;

                leftPivotPoint = result[lastLeftCard];
                rightPivotPoint = result[firstRightCard];

                currentOffset += deltaOffset;
                
                Vector2 indentForStartCards = new Vector3(Math.Abs(_settings.Indent.x / 2), 0);
                var position = _settings.Origin.position;
               
                leftPivotPoint.Position = position - (Vector3)indentForStartCards;
                RotatePoint(leftPivotPoint, currentOffset);
                
                rightPivotPoint.Position = position + (Vector3)indentForStartCards;
                RotatePoint(rightPivotPoint, -currentOffset);

            }
            else
            {
                int centralCard = cardsCount / 2;
                float deltaHeightAlignments = _settings.Indent.y / 1.5f;  
                lastLeftCard = firstRightCard = centralCard;
                
                leftPivotPoint = result[lastLeftCard];
                rightPivotPoint = result[firstRightCard];

                result[centralCard].Position = _settings.Origin.position - new Vector3(0,deltaHeightAlignments);

                sideCardsCount++;
            }
            
          
            for (var i = 1; i < sideCardsCount; i++)
            {
                currentOffset += deltaOffset;
                
                // put leftSide
                result[lastLeftCard - i].Position = leftPivotPoint.Position + indentForLeftCards;
                RotatePoint(result[lastLeftCard - i], currentOffset);
                leftPivotPoint = result[lastLeftCard - i];

                // put rightSide
                result[firstRightCard + i].Position = rightPivotPoint.Position + indentForRightCards;
                RotatePoint(result[firstRightCard + i], -currentOffset);
                rightPivotPoint = result[firstRightCard + i];
            }

            return result;
        }

        private void RotatePoint(CardPosition cardPosition, int angle)
        {
            var euler = cardPosition.Rotation.eulerAngles;
            cardPosition.Rotation.eulerAngles = new Vector3(euler.x, euler.y, euler.z + angle);
        }
        
        public class CardPosition
        {
            public Vector3 Position = Vector3.zero;
            public Quaternion Rotation = Quaternion.identity;
        }
        
        [Serializable] public class Settings
        {
            [SerializeField] private Transform _originPosition;
            [SerializeField] private int _offsetAngle;
            [SerializeField] private Vector2 _indent;
            
            public Transform Origin => _originPosition;
            public int OffsetAngle => _offsetAngle;
            public Vector2 Indent => _indent;
        }
    }
}