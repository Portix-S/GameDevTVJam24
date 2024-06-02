using TMPro;
using UnityEngine;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerJoinText;
    private bool playerJoined;
    
    private void Start()
    {
        playerJoinText.text = "Spacebar/A/X to Join!";
    }
    
    public void Join(int playerIndex)
    {
        playerJoinText.text = "Player " + playerIndex + " is Ready!";
        playerJoined = true;
    }
    
    public void Leave()
    {
        playerJoinText.text = "Spacebar/A/X to Join!";
        playerJoined = false;
    }
}
