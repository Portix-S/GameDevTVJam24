using System;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Mushroom : MonoBehaviour
{
    [SerializeField] string PlayerTag = "Player";
    [SerializeField] float _animationDuration = 0.5f;
    [SerializeField] float _shakeStrength = 0.3f;
    [SerializeField] float _shakeDuration = 2.0f;

    private Vector3 _initialScale;

    public event Action OnPlayerEntered;
    public event Action OnPlayerExit;

    public event Action OnFinishedAnimating;

    private void Awake()
    {
        _initialScale = this.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
            OnPlayerEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
            OnPlayerExit?.Invoke();
    }

    public void Grow(float delay)
    {
        Invoke(nameof(Grow), delay);
    }

    public void Shrink(float delay)
    {
        Invoke(nameof(Shrink), delay);
    }

    public void Grow()
    {
        this.transform.localScale = Vector3.zero;
        SetChildrenActive(true); 
        this.transform.DOScale(_initialScale, _animationDuration).SetEase(Ease.OutBack)
            .OnComplete(() => {
                OnFinishedAnimating?.Invoke();
            }
        );
    }

    public void Shrink()
    {
        DOTween.Sequence()
            .Append(this.transform.DOShakePosition(_shakeDuration, _shakeStrength, fadeOut: false))
            .Append(this.transform.DOScale(Vector3.zero, _animationDuration).SetEase(Ease.InBack))
            .OnComplete( () => { 
                SetChildrenActive(false); 
                OnFinishedAnimating?.Invoke();
            }
        );
    }

    private void SetChildrenActive(bool newActive)
    {
        foreach (Transform child in this.transform)
            child.gameObject.SetActive(newActive);
    }

#if UNITY_EDITOR
    [Button]
    public void Activate()
    {
        this.gameObject.SetActive(true);

        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
            EditorUtility.SetDirty(child);
        }
            
    }


    [Button]
    public void Deactivate()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
            EditorUtility.SetDirty(child);
        }
    }
#endif
}
