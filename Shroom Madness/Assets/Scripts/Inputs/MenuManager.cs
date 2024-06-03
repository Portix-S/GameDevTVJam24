
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inputsMenu;
    [SerializeField] private GameObject reconnectMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private TextMeshProUGUI victoryText;
    private GameObject currentMenu;
    
    private InputManager inputManager;
    private PlayerInputManager playerInputManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        mainMenu.SetActive(true);
        inputsMenu.SetActive(false);
        reconnectMenu.SetActive(false);
        victoryMenu.SetActive(false);
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
    
    public void OpenReconnectMenu()
    {
        currentMenu.SetActive(false);
        reconnectMenu.SetActive(true);
        currentMenu = reconnectMenu;
        inputManager.PauseGame();
    }
    
    public void CloseReconnectMenu()
    {
        currentMenu.SetActive(false);
        currentMenu = mainMenu;
        inputManager.ResumeGame();
    }

    public void MainMenu()
    {
        // Reload Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        inputManager.LeaveGame();
        Back();
    }
    
    public void OpenVictoryMenu(string winnerName)
    {
        currentMenu.SetActive(false);
        victoryMenu.SetActive(true);
        currentMenu = victoryMenu;
        
        victoryText.text = winnerName + " wins!";
    }
}
