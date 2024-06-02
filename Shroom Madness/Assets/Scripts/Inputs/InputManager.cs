using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] private Transform[] spawns;
    [SerializeField] private GameObject inputUI;

    [SerializeField] private GameObject[] playersReadyMenu;
    [SerializeField] private int[] availablePlacesToJoin;
    private int firstAvailablePlace;
    private int playersReady;
    private PlayerInputManager playerInputManager;
    private List<GameObject> players = new();

    [SerializeField] private GameObject playButton;
    private MushroomSlotManager mushroomSlotManager;
    
    // Basic Singleton pattern and variable initialization
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            playerInputManager = GetComponent<PlayerInputManager>();
            availablePlacesToJoin = new int[playerInputManager.maxPlayerCount];
            for (int i = 0; i < availablePlacesToJoin.Length; i++)
            {
                availablePlacesToJoin[i] = -1;
            }
            mushroomSlotManager = FindObjectOfType<MushroomSlotManager>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Set the player's parent to this object, renames the player and deactivates the input
        playerInput.transform.SetParent(this.gameObject.transform);
        playerInput.name = "Player " + (playerInput.playerIndex + 1);
        // playerInput.DeactivateInput();
        playerInput.GetComponent<PlayerMovementTest>().Joined();
        players.Add(playerInput.gameObject);

        // Adds the player to the available places to join
        availablePlacesToJoin[firstAvailablePlace] = playerInput.playerIndex;
        playersReadyMenu[firstAvailablePlace].GetComponent<PlayerJoin>().Join(firstAvailablePlace+1);

        playersReady++;
        if (playersReady >= 2)
        {
            playButton.SetActive(true);
        }
        FindFirstAvailablePlace();
    }
    
    private void FindFirstAvailablePlace()
    {
        for (int i = 0; i < availablePlacesToJoin.Length; i++)
        {
            if (availablePlacesToJoin[i] == -1)
            {
                firstAvailablePlace = i;
                break;
            }
        }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // Removes the player from the available places to join
        for (int i = 0; i < availablePlacesToJoin.Length; i++)
        {
            if (availablePlacesToJoin[i] == playerInput.playerIndex)
            {
                availablePlacesToJoin[i] = -1;
                playersReadyMenu[i].GetComponent<PlayerJoin>().Leave();
                FindFirstAvailablePlace();
                break;
            }
        }
        playersReady--;
        if (playersReady < 2)
        {
            playButton.SetActive(false);
        }
    }

    public void Play()
    {
        // Disables new players from joining after the game started
        playerInputManager.DisableJoining();
        
        // Activates the input for all players and sets their position to the correct spawns
        foreach (var input in PlayerInput.all)
        {
            // input.ActivateInput();
            input.GetComponent<PlayerMovementTest>().Playing();
            input.GetComponent<MeshRenderer>().enabled = true;
            input.GetComponent<Collider>().enabled = true;
            // input.GetComponent<Rigidbody>().isKinematic = false;
            input.transform.position = spawns[input.playerIndex].position;
        }
        
        // Deactivates the input UI
        inputUI.SetActive(false);
        
        // Start Mushroom Slot Manager logic
        FindObjectOfType<MushroomSlotManager>().Initialize();
        
        // Find all players
        // players = new List<GameObject>();
        
    }

    public void PauseGame()
    {
        mushroomSlotManager.Stop();
        foreach (var input in PlayerInput.all)
        {
            input.DeactivateInput();
        }
    }
    
    public void ResumeGame()
    {
        mushroomSlotManager.Initialize();
        foreach (var input in PlayerInput.all)
        {
            input.ActivateInput();
        }
    }

    public void LeaveGame()
    {
        // Destroys all players and resets the available places to join
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
    }
    
}
