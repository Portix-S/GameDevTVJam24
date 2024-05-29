using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MushroomSlotManager : MonoBehaviour
{
    [SerializeField] List<MushroomSlot> _mushroomSlots;


#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] int _difficultyToSet = 0;
    [Button]
    public void SetAllSlotsToDifficulty()
    {
        foreach (var slot in _mushroomSlots)
            slot.SetDifficulty(_difficultyToSet);
    }

    [Button]
    public void GetMushroomSlots()
    {
        _mushroomSlots.Clear();

        foreach (Transform child in this.transform)
            if (child.TryGetComponent<MushroomSlot>(out var slot))
                _mushroomSlots.Add(slot);
    }

    [Button]
    public void GetAllNeededReferencesRecursively()
    {
        GetMushroomSlots();

        foreach (var slot in _mushroomSlots)
            slot.GetAllNeededReferencesRecursively();
    }
#endif
}
