using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InteractionUI interactionUI;
    [SerializeField] private LayerMask interactableLayerMask;

    private readonly Collider[] hitBuffer = new Collider[32];

    private PlayerController playerController;
    private PlayerStats playerStats;
    private IInteractable currentInteractable;

    private bool isItemHovering;
    private bool isHolding;
    private float holdTimer;

    public bool HasInteractable => currentInteractable != null;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponentInChildren<PlayerStats>();
    }

    private void Update()
    {
        FindInteractable();
        UpdateHoldTimer();
    }

    public void SetItemHovering(bool state)
    {
        isItemHovering = state;

        if (!isItemHovering) return;

        holdTimer = 0f;
        isHolding = false;
        interactionUI.Hide();
    }

    public void SetHolding(bool state)
    {
        isHolding = state;

        if (isHolding) return;

        holdTimer = 0f;
        interactionUI.SetProgress(0f);
    }

    private void FindInteractable()
    {
        IInteractable previousInteractable = currentInteractable;
        currentInteractable = null;

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, playerStats.InteractDistance, hitBuffer,
            interactableLayerMask, QueryTriggerInteraction.Collide);

        Vector3 playerPosition = transform.position;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = hitBuffer[i];

            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null || !interactable.CanInteract(playerController)) continue;

            float distance = (hit.transform.position - playerPosition).sqrMagnitude;
            if (distance >= closestDistance) continue;

            closestDistance = distance;
            currentInteractable = interactable;
        }

        if (previousInteractable != currentInteractable) holdTimer = 0f;

        if (currentInteractable != null && !isItemHovering) interactionUI.Show(currentInteractable.UIPosition);
        else interactionUI.Hide();
    }

    private void UpdateHoldTimer()
    {
        if (!isHolding || currentInteractable == null)
        {
            interactionUI.SetProgress(0f);
            return;
        }
        holdTimer += Time.deltaTime;

        float progress = holdTimer / currentInteractable.HoldTime;
        interactionUI.SetProgress(progress);

        if (holdTimer < currentInteractable.HoldTime) return;

        currentInteractable.Interact(playerController);

        holdTimer = 0f;
        isHolding = false;
        interactionUI.SetProgress(0f);
    }
}