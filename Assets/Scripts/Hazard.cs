using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public GameManager gameManager;

    void Start() {
        
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == 6) {
            //signal that it's time to change to score state
            if (gameManager.GetState() is GameManagerRaceState) {
                gameManager.GetStateInput().gameManagerController.respawnPlayer();
                
            }
        }

    }
}
