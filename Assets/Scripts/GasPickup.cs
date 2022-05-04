using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 6)
        {
            StatePlayerController playerController = other.GetComponent<StatePlayerController>();
            if (playerController != null)
            {
                playerController.addGas();
                //Destroy(this.gameObject);
            }
        }
    }
}
