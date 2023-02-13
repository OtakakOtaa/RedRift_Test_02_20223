using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.CardImages
{
    public class CardImagesProvider
    {
        private readonly RandomImageCardLoader _imageLoader;
        private readonly ICoroutineStarter _coroutineStarter;
        
        private Sprite[] _images;
        private int _currentLoadedImages;
        
        public event Action ImagesPrepared;

        public CardImagesProvider(RandomImageCardLoader imageLoader, ICoroutineStarter coroutineStarter)
        {
            _imageLoader = imageLoader;
            imageLoader.ImageLoaded += OnImageLoaded;
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerable<Sprite> Images => _images;

        public void LoadCards(int amount)
        {
            _images = new Sprite[amount];
            
            for (int i = 0; i < amount; i++)
                _coroutineStarter.Start(_imageLoader.LoadImage());
        }
        
        private void OnImageLoaded(Sprite sprite)
        {
            _images[_currentLoadedImages++] = sprite;
            bool isAllCardImageLoaded = _images.Last() is not null;
            if(isAllCardImageLoaded) ImagesPrepared?.Invoke();
        }
    }
}