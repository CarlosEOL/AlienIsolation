using System;
using UnityEngine;

namespace Interactable
{
    public class Interactables : MonoBehaviour, IInteractables
    {
        [SerializeField] public string itemName;

        private void Awake()
        {
            gameObject.name = itemName;
        }

        public virtual string GetName()
        {
            return itemName;
        }

        public virtual void Interact(Controller controller)
        {
            controller.audioClip.Play();
        }
    }
}