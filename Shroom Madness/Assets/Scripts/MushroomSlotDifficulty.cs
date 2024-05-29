using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class MushroomSlotDifficulty : MonoBehaviour
{
    [SerializeField] List<Mushroom> _mushrooms;
    [SerializeField] float _maxActivationDelay = 0.5f;

    
    [ReadOnly][SerializeField]
    private bool _isActive;
    private int _remainingMushroomsToAnimate;


    public bool IsActive => _isActive;

    public event Action OnPlayerEntered;
    public event Action OnPlayerExit;

    private void OnEnable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered += PlayerEntered;
            mushroom.OnPlayerExit += PlayerExit;
            mushroom.OnFinishedAnimating += FinishedStateTransition;
        }
    }

    private void OnDisable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered -= PlayerEntered;
            mushroom.OnPlayerExit -= PlayerExit;
            mushroom.OnFinishedAnimating -= FinishedStateTransition;
        }
    }

    private void FinishedStateTransition()
    {
        _isActive = !_isActive;
        // TODO: Event?
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
        _isActive = true;
        GrowMushrooms();
    }

    public void Deactivate()
    {
        ShrinkMushrooms();

    }

    private void ReallyDeactivate()
    {
        _isActive = false;
        ShrinkMushrooms();
    }

    private void GrowMushrooms()
    {
        _remainingMushroomsToAnimate = _mushrooms.Count;

        foreach (var mushroom in _mushrooms)
        {
            float delay = UnityEngine.Random.Range(0, _maxActivationDelay);
            mushroom.Grow(delay);
        }
    }

    private void ShrinkMushrooms()
    {   
        _remainingMushroomsToAnimate = _mushrooms.Count;

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
    }

    [Button]
    public void EditorActivate()
    {
        _isActive = true;
        
        foreach (var mushroom in _mushrooms)
            mushroom.Activate();
    }

    [Button]
    public void EditorDeactivate()
    {
        _isActive = false;
        
        foreach (var mushroom in _mushrooms)
            mushroom.Deactivate();
    }
#endif

}
