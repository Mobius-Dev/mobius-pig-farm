using UnityEngine;
using System;
using DG.Tweening;

namespace PigFarm
{
    public class Fruit : MonoBehaviour
    {
        public event Action<Fruit> OnFruitRipened;
        
        [Header("Color Settings")]
        [SerializeField] private Color unripeColor = Color.green;
        [SerializeField] private Color ripeColor = Color.red;
        [SerializeField] private float ripeningDuration = 5f;
        [SerializeField] private float ripeningDurationMin = 4.0f;
        [SerializeField] private float ripeningDurationMax = 6.0f;
        
        [Header("Renderer Settings")]
        [SerializeField] private Renderer fruitRenderer;
        [SerializeField] private string colorPropertyName = "_BaseColor"; // For URP use "_BaseColor" else "_Color"

        [Header("Physics Settings")]
        [SerializeField] private Rigidbody rigidBody;

        [Header("Debug")]
        [SerializeField] private Color currentColor;
        [SerializeField] private float ripeningProgress = 0f;
        
        private Tween ripeningTween;
        private MaterialPropertyBlock propertyBlock;

        

        void Awake()
        {
            // Auto-get renderer if not assigned
            if (fruitRenderer == null)
                fruitRenderer = GetComponent<Renderer>();
            if (rigidBody == null)
                rigidBody = GetComponent<Rigidbody>();
                
            propertyBlock = new MaterialPropertyBlock();
        }

        void Start()
        {
            currentColor = unripeColor;
            StartRipening();
        }
        
        void Update()
        {
            SetColor();
        }
        
        private void SetColor()
        {
            // Apply color using MaterialPropertyBlock (doesn't create material instances)
            fruitRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorPropertyName, currentColor);
            fruitRenderer.SetPropertyBlock(propertyBlock);
        }
        
        private void StartRipening()
        {
            ripeningTween?.Kill();
            
            // Tween the progress value
            ripeningTween = DOTween.To(() => ripeningProgress, x => ripeningProgress = x, 1f, ripeningDuration)
                .SetEase(Ease.InOutQuad)
                .OnUpdate(() => {
                    // Blend colors based on progress
                    currentColor = Color.Lerp(unripeColor, ripeColor, ripeningProgress);
                })
                .OnComplete(() => {
                    Debug.Log("Fruit ripened!");
                    OnRipeningComplete();
                });
        }

        private void OnRipeningComplete()
        {
            // Kill tween and clean up
            ripeningTween?.Kill();
            ripeningTween = null;

            rigidBody.useGravity = true;
            OnFruitRipened?.Invoke(this);
        }
        
        void OnDestroy()
        {
            ripeningTween?.Kill();
        }
    }
}