
using UnityEngine;

namespace Interactable
{
    public interface IInteractables
    {
        public string GetName();
        public void Interact(Controller controller);
    }
}