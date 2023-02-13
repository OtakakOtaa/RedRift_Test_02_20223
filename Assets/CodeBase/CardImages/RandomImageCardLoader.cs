using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CodeBase.CardImages
{
    public class RandomImageCardLoader
    {
        private readonly Settings _settings;

        public event Action<Sprite> ImageLoaded; 
        
        public RandomImageCardLoader(Settings settings)
        {
            _settings = settings;
        }
        
        public IEnumerator LoadImage()
        {
            var webRequest = UnityWebRequestTexture.GetTexture(BuildRequest());
            yield return webRequest.SendWebRequest();

            Texture2D receivedTexture = DownloadHandlerTexture.GetContent(webRequest);
            ImageLoaded?.Invoke(ConvertToSprite(receivedTexture));
        }
        
        private string BuildRequest()
            => $"{_settings.LoadOrigin}/{_settings.ResolutionX}/{_settings.ResolutionY}";

        private Sprite ConvertToSprite(Texture2D texture)
        {
            var rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(texture, rect, pivot);
        }
        
        [Serializable] public class Settings
        { 
            [SerializeField] private Vector2 _resolution;
            public string LoadOrigin; 
            
            public int ResolutionX => (int)_resolution.x;
            public int ResolutionY => (int)_resolution.y;
        } 
    }
}
