using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    public ClawGrabberMinigame clawGrabberMinigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderWash")
        {
            clawGrabberMinigame.ChangeMovingSide();
        }

        if (collision.gameObject.tag == "BottomWash")
        {
            clawGrabberMinigame.Return();

        }

        if (collision.gameObject.tag == "Clothes")
        {
            

            if (clawGrabberMinigame.IsGrabbing())
            {
                Debug.Log($"Collided with {collision.gameObject.name}");
                clawGrabberMinigame.GrabbedCloth(collision.gameObject);
                clawGrabberMinigame.Return();
            }
           


        }
    }
}
