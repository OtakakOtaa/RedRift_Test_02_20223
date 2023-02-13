using CodeBase.Hand.View;
using UnityEngine;

namespace CodeBase.DragAndDrop
{
    public class CardHolder
    {
        private const float MaxOffset = 0.5f;

        private readonly Transform _position;

        public CardHolder(Transform position)
        {
            _position = position;
        }
        
        public Vector3 Position => _position.position;
        
        public bool TryCanHoldCard(CardView cardView)
        {
            bool isCardNear = Vector3.Distance(Position, cardView.transform.position) <= MaxOffset;
            if (isCardNear)
            {
                var transform = cardView.transform;
                transform.position = Position;
                transform.rotation = _position.rotation;
            }

            return isCardNear;
        }
        
    }
}