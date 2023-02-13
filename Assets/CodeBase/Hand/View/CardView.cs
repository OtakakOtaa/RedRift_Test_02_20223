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

        [SerializeField] private Material _glowFrameMaterial;
        private GlowFrameShader _glowFrameShader;


        public event Action InitializeEnded;

        public int LayerComponentsCount => 4;

        public GrippeableObject GrippeableObject { get; private set; }

        public void Start()
        {
            _canvas.worldCamera = Camera.current;

            GrippeableObject = GetComponent<GrippeableObject>();
            
            _glowFrameShader = new GlowFrameShader();
            DuplicateMaterial();
            GrippeableObject.StartCaptured += EnableFrameGlow;
            GrippeableObject.StopCaptured += DisableFrameGlow;

            InitializeEnded?.Invoke();
        }

        private void EnableFrameGlow(Transform card)
            => _glowFrameMaterial.SetFloat(_glowFrameShader.PowerValueId, _glowFrameShader.OnEnableGlowAmount);

        private void DisableFrameGlow()
            => _glowFrameMaterial.SetFloat(_glowFrameShader.PowerValueId, _glowFrameShader.OnDisableGlowAmount);

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

        private void DuplicateMaterial()
        {
            _glowFrameMaterial = new Material(_glowFrameMaterial);
            _frame.material = _glowFrameMaterial;
        }
        
        private const string HealthStateHeader = "Health : ";
        private const string ManaCostStateHeader = "Manacost : ";
        private const string AttackStateHeader = "Attack : ";

        class GlowFrameShader
        {
            public int PowerValueId => Shader.PropertyToID("_Flutness");
            public float OnEnableGlowAmount => 0.009f;
            public float OnDisableGlowAmount => default;
        }
    }
}