using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] private Transform[] spawns;
    [SerializeField] private GameObject inputUI;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.SetParent(this.gameObject.transform);
        playerInput.name = "Player " + (playerInput.playerIndex + 1);
        playerInput.DeactivateInput();
    }

    public void Play()
    {
        foreach (var input in PlayerInput.all)
        {
            input.ActivateInput();
            input.GetComponent<MeshRenderer>().enabled = true;
            input.GetComponent<Collider>().enabled = true;
            input.GetComponent<Rigidbody>().isKinematic = false;
            input.transform.position = spawns[input.playerIndex].position;
        }
        inputUI.SetActive(false);
    }


}
