using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MushroomSlotManager : MonoBehaviour
{
    [SerializeField] List<MushroomSlot> _mushroomSlots;

    [SerializeField] float _minDifficultUpdateCooldown = 5f;
    [SerializeField] float _maxDifficultUpdateCooldown = 15;

    [SerializeField] int _minDifficultChange = -1;
    [SerializeField] int _maxDifficultChange = 3;

    [SerializeField] float _maxDesiredDifficulty = 3.3f;

    private int MinDifficulty => _mushroomSlots.Count;

    private int DifficultySum
    {
        get
        {
            int sum = 0;
            foreach (var slot in _mushroomSlots)
                sum += slot.DifficultyAfterAnimation;

            return sum;
        }
    }

    private void Awake()
    {
        // Initialize();
    }
    
    public void Initialize()
    {
        StartCoroutine(DifficultyUpdateLoop());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    private IEnumerator DifficultyUpdateLoop()
    {
        float cooldown = Random.Range(_minDifficultUpdateCooldown, _maxDifficultUpdateCooldown);
        yield return new WaitForSeconds(cooldown);

        int change = Random.Range(_minDifficultChange, _maxDifficultChange + 1);

        int difficulty = DifficultySum;

        while (change > 0 && 
              (float)(difficulty + change) / _mushroomSlots.Count > _maxDesiredDifficulty)
            change--;

        while (change < 0 && 
              (float)(difficulty + change) / _mushroomSlots.Count < MinDifficulty)
            change++;

        int slotsToIncreaseDifficulty = 0;
        int slotsToDecreaseDifficulty = 0;

        if (change > 0)
            slotsToIncreaseDifficulty = change;
        else if (change < 0)
            slotsToDecreaseDifficulty = change;
        else
        {
            slotsToIncreaseDifficulty = 1;
            slotsToDecreaseDifficulty = 1;
        }

        for (int i = 0; i < slotsToIncreaseDifficulty; i++)
        {
            var slot = SelectSlotToIncreaseDifficulty();
            if (slot != null)
                slot.IncreaseDifficulty();
        }

        for (int i = 0; i < slotsToDecreaseDifficulty; i++)
        {
            var slot = SelectSlotToDecreaseDifficulty();
            if (slot != null)
                slot.DecreaseDifficulty();
        }

        StartCoroutine(DifficultyUpdateLoop());
    }
    
    private MushroomSlot SelectSlotToIncreaseDifficulty()
    {
        var candidates = _mushroomSlots.FindAll(x => x.CanIncreaseDifficulty);
        if (candidates.Count == 0)
            return null;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

    private MushroomSlot SelectSlotToDecreaseDifficulty()
    {
        var candidates = _mushroomSlots.FindAll(x => x.CanDecreaseDifficulty);
        if (candidates.Count == 0)
            return null;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] int _difficultyToSet = 0;
    [Button]
    public void SetAllSlotsToDifficulty()
    {
        foreach (var slot in _mushroomSlots)
            slot.SetDifficulty(_difficultyToSet);

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void GetMushroomSlots()
    {
        _mushroomSlots.Clear();

        foreach (Transform child in this.transform)
            if (child.TryGetComponent<MushroomSlot>(out var slot))
                _mushroomSlots.Add(slot);

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void GetAllNeededReferencesRecursively()
    {
        GetMushroomSlots();

        foreach (var slot in _mushroomSlots)
            slot.GetAllNeededReferencesRecursively();
        
        EditorUtility.SetDirty(this);
    }
#endif
}
