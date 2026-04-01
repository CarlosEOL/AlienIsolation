using System;
using Interactable;
using UnityEngine;

public class Health : Interactables
{
    [SerializeField] public SphereCollider col;
    [SerializeField] public float healthAmt;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Controller>().AddHealth(healthAmt);
            Destroy(gameObject);
        }
    }

    public override void Interact(Controller controller)
    {
        controller.AddHealth(healthAmt);
        Destroy(gameObject);
    }
}
