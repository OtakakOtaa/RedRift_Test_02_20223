using System;
using CodeBase.DragAndDrop;
using CodeBase.Hand.Model;
using TMPro;
using UnityEngine;

namespace CodeBase.Hand.View
{
    [RequireComponent(typeof(DragAndDrop.GrippeableObject))]
    public class CardView : MonoBehaviour, ILayout
    {
        [SerializeField] private TextMeshProUGUI _healthState;
        [SerializeField] private TextMeshProUGUI _manaCostState;
        [SerializeField] private TextMeshProUGUI _attackState;
        
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        [SerializeField] private SpriteRenderer _image;
        [SerializeField] private SpriteRenderer _frame;
        [SerializeField] private SpriteRenderer _header;
        [SerializeField] private Canvas _canvas;

        public event Action InitializeEnded;
        
        public int LayerComponentsCount => 4;

        public GrippeableObject GrippeableObject { get; private set; }

        public void Start()
        {
            _canvas.worldCamera = Camera.current;
            GrippeableObject = GetComponent<GrippeableObject>();
            InitializeEnded?.Invoke();
        }

        public void UpdateParam(Card card)
        {
            _healthState.text = HealthStateHeader + card.Health;
            _manaCostState.text = ManaCostStateHeader + card.ManaCost;
            _attackState.text = AttackStateHeader + card.Attack;
        }

        public void SetImage(Sprite image)
            => _image.sprite = image;
        
        public void SetLayerOrder(int orderAmount)
        {
            _image.sortingOrder = orderAmount++;
            _frame.sortingOrder = orderAmount++;
            _header.sortingOrder = orderAmount++;
            _canvas.sortingOrder = orderAmount;
        }
        
        private const string HealthStateHeader = "Health : ";
        private const string ManaCostStateHeader = "Manacost : ";
        private const string AttackStateHeader = "Attack : ";
    }
}