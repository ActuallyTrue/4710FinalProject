using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManagerController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject playerPrefab;
    public Transform spawnPoint;  

    public GameObject hazardPrefab; 

    private StatePlayerController playerController;

    public List<Vector3> jumpPositions;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        jumpPositions = new List<Vector3>();
    }

    void Start() {
        playerController = FindObjectOfType<StatePlayerController>();
    }

    //just teleports the player back to their original spawnpoint
    //make this function run a death animation on the player and have the player get teleported after the animation
    public void respawnPlayer() {
        if (!playerController.professorMode) {
            playerController.stopGrapple();
            playerController.gameObject.transform.position = spawnPoint.position;
        }  
    }

    public void endGame() {
            Destroy(playerController.gameObject);
            foreach (Rewired.Player player in Rewired.ReInput.players.Players) {
                player.controllers.ClearAllControllers();
            }
            //SceneManager.LoadScene("Launcher");
    }

    public void recordJump(Vector3 position) {
        jumpPositions.Add(position);
    }

    public void spawnHazards() {
        foreach (Vector3 position in jumpPositions) {
            Instantiate(hazardPrefab, position, Quaternion.identity);
        }

        jumpPositions.Clear();
    }

    

}
