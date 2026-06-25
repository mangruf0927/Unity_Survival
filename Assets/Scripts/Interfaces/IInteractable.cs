using UnityEngine;

public interface IInteractable
{
    float HoldTime { get; }
    Vector3 UIPosition { get; }

    bool CanInteract(PlayerController player);
    void Interact(PlayerController player);
}
