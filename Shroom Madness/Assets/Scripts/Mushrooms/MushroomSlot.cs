using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;


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
    private int _currentDifficultyIndex;
    private int _nextDifficultyIndex;
    private bool _isTransitioningSmoothly;

    public event Action<int> OnUpdatedPlayerCount;

    public int CurrentDifficulty => _currentDifficultyIndex + 1;
    public int DifficultyAfterAnimation => _nextDifficultyIndex + 1;
    public int MaxDifficulty => _difficulties.Count;

    public bool IsOnMinDifficulty => CurrentDifficulty == 1;
    public bool IsOnMaxDifficulty => CurrentDifficulty == MaxDifficulty;
    public bool IsAnimating => _isTransitioningSmoothly || _difficulties.Any(x => x.IsAnimating);
    public bool CanIncreaseDifficulty => !IsOnMaxDifficulty && !IsAnimating;
    public bool CanDecreaseDifficulty => !IsOnMinDifficulty && !IsAnimating;
    

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
        ChangeDifficultyTo(_currentDifficultyIndex + 1);
    }

    public void DecreaseDifficulty()
    {
        ChangeDifficultyTo(_currentDifficultyIndex - 1);
    }

    private void ChangeDifficultyTo(int newDifficultyIndex)
    {
        if (newDifficultyIndex < 0 || newDifficultyIndex >= _difficulties.Count)
            return;

        _nextDifficultyIndex = _currentDifficultyIndex;

        if (_difficulties[_currentDifficultyIndex].IsActive)
        {
            _isTransitioningSmoothly = true;
            StartCoroutine(TransitionDifficultSmoothlyTo(newDifficultyIndex));
        }
        else
            ActivateNewDifficulty(newDifficultyIndex);
    }

    private IEnumerator TransitionDifficultSmoothlyTo(int newDifficultyIndex)
    {

        _difficulties[_currentDifficultyIndex].Deactivate();

        yield return new WaitUntil(() => !_difficulties[_currentDifficultyIndex].IsActive);
        yield return new WaitForSeconds(_difficultyTransitionDelay);

        _isTransitioningSmoothly = false;

        ActivateNewDifficulty(newDifficultyIndex);
    }

    private void ActivateNewDifficulty(int newDifficulty)
    {
        _playerCount = 0;

        _currentDifficultyIndex = newDifficulty;
        _nextDifficultyIndex = _currentDifficultyIndex;
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

        _currentDifficultyIndex = _difficultyToSet;

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
