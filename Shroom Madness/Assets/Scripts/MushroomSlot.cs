using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MushroomSlot : MonoBehaviour
{
    [SerializeField] List<MushroomSlotDifficulty> _difficulties = new List<MushroomSlotDifficulty>();
    [SerializeField] float _difficultyTransitionDelay = 1.0f;

    [ReadOnly][SerializeField]
    private int _playerCount;
    
    [ReadOnly][SerializeField]
    private int _currentDifficulty;

    public event Action<int> OnUpdatedPlayerCount;

    public int CurrentDifficulty => _currentDifficulty;

    public int PlayerCount 
    {
        get => _playerCount;
        private set 
        {
            _playerCount = value;
            OnUpdatedPlayerCount?.Invoke(_playerCount);
        }
    }

    private void OnEnable()
    {
        foreach (var difficulty in _difficulties)
        {
            difficulty.OnPlayerEntered += IncreasePlayerCount;
            difficulty.OnPlayerExit += DecreasePlayerCount;
        }
    }

    private void OnDisable()
    {
        foreach (var difficulty in _difficulties)
        {
            difficulty.OnPlayerEntered -= IncreasePlayerCount;
            difficulty.OnPlayerExit -= DecreasePlayerCount;
        }
    }

    private void IncreasePlayerCount()
    {
        PlayerCount++;
    }

    private void DecreasePlayerCount()
    {
        PlayerCount--;
    }

    public void IncreaseDifficulty()
    {
        ChangeDifficultyTo(CurrentDifficulty + 1);
    }

    public void DecreaseDifficulty()
    {
        ChangeDifficultyTo(CurrentDifficulty - 1);
    }

    private void ChangeDifficultyTo(int newDifficulty)
    {
        if (newDifficulty < 0 || newDifficulty >= _difficulties.Count)
            return;

        if (_difficulties[_currentDifficulty].IsActive)
            StartCoroutine(TransitionDifficultSmoothlyTo(newDifficulty));
        else
            ActivateNewDifficulty(newDifficulty);
    }

    private IEnumerator TransitionDifficultSmoothlyTo(int newDifficulty)
    {
        _difficulties[_currentDifficulty].Deactivate();

        yield return new WaitUntil(() => !_difficulties[_currentDifficulty].IsActive);
        yield return new WaitForSeconds(_difficultyTransitionDelay);

        ActivateNewDifficulty(newDifficulty);
    }

    private void ActivateNewDifficulty(int newDifficulty)
    {
        _playerCount = 0;

        _currentDifficulty = newDifficulty;
        _difficulties[newDifficulty].Activate();
    }
    
#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] int _difficultyToSet = 0;

    [Button]
    public void SetDifficulty()
    {
        for (int i = 0; i < _difficulties.Count; i++)
        {
            if (i == _difficultyToSet)
                _difficulties[i].EditorActivate();
            else
                _difficulties[i].EditorDeactivate();
        }

        EditorUtility.SetDirty(this);
    }

    public void SetDifficulty(int newDifficulty)
    {
        _difficultyToSet = newDifficulty;
        SetDifficulty();

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void GetDifficulties()
    {
        _difficulties.Clear();
        
        foreach (Transform child in this.transform)
            if (child.TryGetComponent<MushroomSlotDifficulty>(out var difficulty))
                _difficulties.Add(difficulty);

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void GetAllNeededReferencesRecursively()
    {
        GetDifficulties();

        foreach (var difficulty in _difficulties)
            difficulty.GetMushroomReferences();

        EditorUtility.SetDirty(this);
    }
#endif
}
