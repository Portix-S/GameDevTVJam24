using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MushroomSlotDifficulty : MonoBehaviour
{
    [SerializeField] List<Mushroom> _mushrooms;
    [SerializeField] float _maxActivationDelay = 0.5f;

    
    [ReadOnly][SerializeField]
    private bool _isActive;
    private bool _isAnimating;
    private int _remainingMushroomsToAnimate;


    public bool IsActive => _isActive;
    public bool IsAnimating => _isAnimating;

    public event Action OnPlayerEntered;
    public event Action OnPlayerExit;

    private void OnEnable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered += PlayerEntered;
            mushroom.OnPlayerExit += PlayerExit;
            mushroom.OnFinishedAnimating += OnMushroomFinishedShrinking;
        }
    }

    private void OnDisable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered -= PlayerEntered;
            mushroom.OnPlayerExit -= PlayerExit;
            mushroom.OnFinishedAnimating -= OnMushroomFinishedShrinking;
        }
    }

    private void FinishedStateTransition()
    {
        _isActive = !_isActive;
        _isAnimating = false;
    }

    private void OnMushroomFinishedShrinking()
    {
        _remainingMushroomsToAnimate--;
        if (_remainingMushroomsToAnimate == 0)
            FinishedStateTransition();
    }

    private void PlayerEntered()
    {
        OnPlayerEntered?.Invoke();
    }

    private void PlayerExit()
    {
        OnPlayerExit?.Invoke();
    }

    public void Activate()
    {
        GrowMushrooms();
    }

    public void Deactivate()
    {
        ShrinkMushrooms();
    }

    private void GrowMushrooms()
    {
        _remainingMushroomsToAnimate = _mushrooms.Count;

        if (_remainingMushroomsToAnimate == 0)
        {
            FinishedStateTransition();
            return;
        }

        _isAnimating = true;

        foreach (var mushroom in _mushrooms)
        {
            float delay = UnityEngine.Random.Range(0, _maxActivationDelay);
            mushroom.Grow(delay);
        }
    }

    private void ShrinkMushrooms()
    {   
        _remainingMushroomsToAnimate = _mushrooms.Count;

        if (_remainingMushroomsToAnimate == 0)
        {
            FinishedStateTransition();
            return;
        }

        _remainingMushroomsToAnimate = _mushrooms.Count;
        _isAnimating = true;

        foreach (var mushroom in _mushrooms)
        {
            float delay = UnityEngine.Random.Range(0, _maxActivationDelay);
            mushroom.Shrink(delay);
        }
    }

#if UNITY_EDITOR
    [Button]
    public void GetMushroomReferences()
    {
        _mushrooms.Clear();
        
        foreach (Transform child in this.transform)
            if (child.TryGetComponent<Mushroom>(out var mushroom))
                _mushrooms.Add(mushroom);

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void EditorActivate()
    {
        _isActive = true;
        
        foreach (var mushroom in _mushrooms)
            mushroom.Activate();

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void EditorDeactivate()
    {
        _isActive = false;
        
        foreach (var mushroom in _mushrooms)
            mushroom.Deactivate();

        EditorUtility.SetDirty(this);
    }
#endif

}
