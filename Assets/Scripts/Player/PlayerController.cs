using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform equipPosition;
    [SerializeField] private Rigidbody rigid;

    public Animator animator;

    private Vector2 moveDirection;
    private Vector3 lookDirection = Vector3.forward;
    
    private bool isRun;
    private bool isGround;

    private EquippableItem currentEquipped;
    private Weapon currentWeapon;
    public Weapon CurrentWeapon => currentWeapon;
    private Sack currentSack;

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetAimPoint(Vector3 point) { currentWeapon.SetAimPoint(point); }
    public void SetRun(bool state) { isRun = state; }
    public void SetWeapon(Weapon weapon) { currentWeapon = weapon; }
    public void SetSack(Sack sack) { currentSack = sack; }

    public bool IsRun() { return isRun; }
    public bool IsGround() { return isGround; }

    public Vector2 GetDirection() { return moveDirection; }
    private Vector3 GetCameraDirection(Vector2 input)
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        return Vector3.ClampMagnitude(direction, 1f);
    }

    public void Move()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        float speed = isRun ? playerStats.runSpeed : playerStats.moveSpeed;

        Vector3 moveVec = GetCameraDirection(moveDirection);
        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }

    public void Look()
    {
        Vector3 lookVec = GetCameraDirection(moveDirection);
        if (lookVec.sqrMagnitude >= 0.0001f) lookDirection = lookVec.normalized;

        Quaternion target = Quaternion.LookRotation(lookDirection, Vector3.up);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target, Time.fixedDeltaTime * playerStats.rotateSpeed));
    }

    public void Stop()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, curVelocity.y, 0f);
    }

    public void Jump()
    {
        isGround = false;
        rigid.AddForce(Vector3.up * playerStats.jumpForce, ForceMode.Impulse);
    }
 
    public bool GetEquippableItem(EquippableItem item)
    {
        if (item == null) return false;
        if (!inventory.AddItem(item, out EquippableItem prevItem)) return false;

        item.Attach(equipPosition);
        item.gameObject.SetActive(false);

        if (ReplacedItem(prevItem, item))
        {
            currentEquipped = item;
            item.OnEquip(this);
            UpdateUpperBodyWeight();
        }
        return true;
    }

    private bool ReplacedItem(EquippableItem prevItem, EquippableItem newItem)
    {
        if (prevItem == null) return false;

        bool equipped = currentEquipped == prevItem;
        if (equipped)
        {
            prevItem.OnUnequip(this);
            currentEquipped = null;
            currentWeapon = null;
            currentSack = null;
        }

        if (prevItem is Sack prevSack && newItem is Sack newSack)
        {
            prevSack.MoveItems(newSack);
        }

        Destroy(prevItem.gameObject);
        return equipped;
    }

    public void DropEquippableItem()
    {
        if (currentEquipped == null || !currentEquipped.CanDrop) return;

        EquippableItem item = currentEquipped;

        item.OnUnequip(this);
        inventory.RemoveItem(item);

        item.gameObject.SetActive(true);
        item.Detach();

        currentEquipped = null;
        currentWeapon = null;
        currentSack = null;

        UpdateUpperBodyWeight();
    }

    public void EquipItem(int idx)
    {
        EquippableItem nextItem = inventory.SelectItem(idx);
        if (nextItem == null) return;

        if (currentEquipped == nextItem)
        {
            UnequipItem();
            return;
        }

        if (currentEquipped != null) currentEquipped.OnUnequip(this);

        currentEquipped = nextItem;
        currentEquipped.OnEquip(this);

        UpdateUpperBodyWeight();
    }

    public void UnequipItem()
    {
        if (currentEquipped == null) return;

        currentEquipped.OnUnequip(this);
        currentEquipped = null;
        currentSack = null;
        currentWeapon = null;

        UpdateUpperBodyWeight();
    }

    public bool GetCollectibleItem(Item item)
    {
        if (currentSack == null) return false;
        return currentSack.AddItem(item);
    }

    public void DropCollectibleItem()
    {
        if (currentSack == null) return;

        Item item = currentSack.DropItem();
        if (item == null) return;

        item.transform.position = transform.position + transform.forward * 1.5f + Vector3.up;
    }

    private void UpdateUpperBodyWeight()
    {
        animator.SetLayerWeight(1, currentEquipped == null ? 0f : 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGround = true;
    }
}