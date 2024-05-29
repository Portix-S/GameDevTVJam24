using System;
using UnityEngine;
using NaughtyAttributes;

public class Mushroom : MonoBehaviour
{
    private const string PlayerTag = "Player";

    public event Action OnPlayerEntered;
    public event Action OnPlayerExit;


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

    [Button]
    public void Grow()
    {
        // TODO: Animation
        this.gameObject.SetActive(true);
    }

    [Button]
    public void Shrink()
    {
        // TODO: Animation
        this.gameObject.SetActive(false);
    }
}
