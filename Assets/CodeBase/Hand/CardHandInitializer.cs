using System;
using System.Collections.Generic;
using CodeBase.CardImages;
using CodeBase.DragAndDrop;
using CodeBase.Hand.Model;
using CodeBase.Hand.View;
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Hand
{
    public class CardHandInitializer
    {
        public event Action<CardsHand, CardHandView> CardHandInitialized;

        public void Initialize(CardImagesProvider cardImagesProvider,
            IDestroyer destroyer, IGameObjectCreator creator,
            Configuration configuration, Transform cardHolder, CardHandView.Settings settings)
        {
            CardHandView cardHandView = null;
            CardsHand cardsHand;

            int cardsCount = configuration.CardsAmountsInHand;

            cardsHand = CreateCardHandModel(configuration, cardsCount);
            cardImagesProvider.LoadCards(cardsCount);

            cardImagesProvider.ImagesPrepared += () =>
                cardHandView = CreateCardHandView(cardsHand,
                    destroyer,
                    creator,
                    cardImagesProvider.Images,
                    cardHolder,
                    settings
                );
            cardImagesProvider.ImagesPrepared += () =>
                CardHandInitialized?.Invoke(cardsHand, cardHandView);
        }

        private CardsHand CreateCardHandModel(Configuration configuration, int cardsCount)
            => new CardsHand(CreateRandomCards(configuration, cardsCount));

        private CardHandView CreateCardHandView(CardsHand cardsHand,
            IDestroyer destroyer, IGameObjectCreator creator,
            IEnumerable<Sprite> images, Transform cardHolder, CardHandView.Settings settings)
            => new CardHandView(cardsHand, images, destroyer, creator, cardHolder, settings);

        private IEnumerable<Card> CreateRandomCards(Configuration configuration, int cardCount)
        {
            var randomCards = new Card[cardCount];
            foreach (var card in randomCards)
                yield return new Card
                (
                    health: configuration.StartCardParam,
                    manaCost: configuration.StartCardParam,
                    attack: configuration.StartCardParam
                );
        }
    }
}