using System.Collections;
using CodeBase.CardImages;
using CodeBase.CardsButton;
using CodeBase.DragAndDrop;
using CodeBase.Hand;
using CodeBase.Hand.View;
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase
{
    public class ProjectInstaller : MonoBehaviour, ICoroutineStarter, IDestroyer, IGameObjectCreator
    {
        [SerializeField] private CardsButtonView _cardsButtonView;
        [SerializeField] private Transform _cardHolderPosition;

        [SerializeField] private RandomImageCardLoader.Settings _cardLoadersSettings; 
        [SerializeField] private CardHandView.Settings _cardHandViewSettings;

        [SerializeField] private Configuration _configuration;

        private Gripper _gripper;
        
        private void Start()
        {
            InitProject();
        }

        private void InitProject()
        {
            RandomImageCardLoader randomImageCardLoader = new(_cardLoadersSettings);
            CardImagesProvider cardImagesProvider = new(randomImageCardLoader, coroutineStarter: this);

            CardHandInitializer cardHandInitializer = new();

            cardHandInitializer.Initialize(
                cardImagesProvider,
                destroyer: this,
                creator: this,
                _configuration,
                _cardHolderPosition,
                _cardHandViewSettings
            );
            cardHandInitializer.CardHandInitialized += (hand, view) =>
            {
                _cardsButtonView.InjectDependencies(hand, _configuration);
                view.PutCards();
                _gripper = new Gripper();
            };
        }

        public void Update()
        {
            _gripper?.Update();
        }
        
        void ICoroutineStarter.Start(IEnumerator coroutine)
            => StartCoroutine(coroutine);

        public void DestroyObject(GameObject target)
            => Destroy(target);

        public GameObject CreateByPrefab(GameObject prefab)
            => Instantiate(prefab);
    }
}