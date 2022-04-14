using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    public GameManager gameManager;
    public GameManagerController gameManagerController;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        gameManagerController = FindObjectOfType<GameManagerController>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("I got hit!!");
        if (collider.gameObject.layer == 6) {
            
            gameManagerController.respawnPlayer();
            // //signal that it's time to change to score state
            // if (gameManager.GetState() is GameManagerRaceState) {
            //     gameManager.GetStateInput().gameManagerController.respawnPlayer();
            //     gameManager.GetStateInput().gameManagerController.jumpPositions.Clear();
            // }
        }

    }
}
