
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inputsMenu;
    private GameObject currentMenu;
    
    private InputManager inputManager;
    private PlayerInputManager playerInputManager;
    private void Start()
    {
        mainMenu.SetActive(true);
        inputsMenu.SetActive(false);
        currentMenu = mainMenu;
        inputManager = GetComponent<InputManager>();
        playerInputManager = GetComponent<PlayerInputManager>();
        inputManager.enabled = false;
        playerInputManager.DisableJoining();
    }
    
    public void Play()
    {
        currentMenu.SetActive(false);
        inputsMenu.SetActive(true);
        currentMenu = inputsMenu;
        
        inputManager.enabled = true;
        playerInputManager.EnableJoining();

    }
    
    public void Back()
    {
        currentMenu.SetActive(false);
        mainMenu.SetActive(true);
        currentMenu = mainMenu;
    }
    
    
}
