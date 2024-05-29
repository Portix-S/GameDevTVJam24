using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MushroomSlotDifficulty : MonoBehaviour
{
    [SerializeField] List<Mushroom> _mushrooms;

    
    [ReadOnly][SerializeField]
    private bool _isActive;

    public bool IsActive => _isActive;

    public event Action OnPlayerEntered;
    public event Action OnPlayerExit;

    private void OnEnable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered += PlayerEntered;
            mushroom.OnPlayerExit += PlayerExit;
        }
    }

    private void OnDisable()
    {
        foreach (var mushroom in _mushrooms)
        {
            mushroom.OnPlayerEntered -= PlayerEntered;
            mushroom.OnPlayerExit -= PlayerExit;
        }
    }

    private void PlayerEntered()
    {
        OnPlayerEntered?.Invoke();
    }

    private void PlayerExit()
    {
        OnPlayerExit?.Invoke();
    }

    [Button]
    public void Activate()
    {
        _isActive = true;
        GrowMushrooms();
    }

    [Button]
    public void Deactivate()
    {
        _isActive = false;
        ShrinkMushrooms();
    }

    private void GrowMushrooms()
    {
        foreach (var mushroom in _mushrooms)
            mushroom.Grow();
    }

    private void ShrinkMushrooms()
    {
        foreach (var mushroom in _mushrooms)
            mushroom.Shrink();
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
#endif

}
