using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Inventory inventory;

    public Transform equipPosition;
    public Animator animator;
    public Weapon currentWeapon;
    public Sack currentSack;

    private EquippableItem currentEquipped;
    private Vector2 moveDirection;
    private Vector3 lookDirection = Vector3.forward;

    private bool isRun;
    private bool isGround;

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetAimPoint(Vector3 point) { currentWeapon.SetAimPoint(point); }
    public void SetRun(bool state) { isRun = state; }

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
        float speed = isRun ? playerStat.runSpeed : playerStat.moveSpeed;

        Vector3 moveVec = GetCameraDirection(moveDirection);
        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }

    public void Look()
    {
        Vector3 lookVec = GetCameraDirection(moveDirection);
        if (lookVec.sqrMagnitude >= 0.0001f) lookDirection = lookVec.normalized;

        Quaternion target = Quaternion.LookRotation(lookDirection, Vector3.up);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target, Time.fixedDeltaTime * playerStat.rotateSpeed));
    }

    public void Stop()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, curVelocity.y, 0f);
    }

    public void Jump()
    {
        isGround = false;
        rigid.AddForce(Vector3.up * playerStat.jumpForce, ForceMode.Impulse);
    }

    public bool GetItem(EquippableItem item)
    {
        if (item == null) return false;

        if (item is MeleeWeapon nextMelee)
            return GetMeleeWeapon(nextMelee);

        if (item is Sack nextSack)
            return GetSack(nextSack);

        if (!inventory.AddItem(item)) return false;

        item.Attach(equipPosition);
        item.gameObject.SetActive(false);
        return true;
    }

    private bool GetMeleeWeapon(MeleeWeapon nextMelee)
    {
        if (nextMelee == null) return false;

        MeleeWeapon prevMelee = null;

        foreach (EquippableItem item in inventory.Items)
        {
            if (item is MeleeWeapon melee && melee.ItemName == nextMelee.ItemName)
            {
                prevMelee = melee;
                break;
            }
        }

        if (prevMelee == null)
        {
            if (!inventory.AddItem(nextMelee)) return false;

            nextMelee.Attach(equipPosition);
            nextMelee.gameObject.SetActive(false);
            return true;
        }

        if (nextMelee.Level <= prevMelee.Level) return false;

        bool wasEquipped = (currentEquipped == prevMelee);

        inventory.RemoveItem(prevMelee);

        if (wasEquipped)
        {
            prevMelee.OnUnequip(this);
            currentEquipped = null;
            currentWeapon = null;
        }

        Destroy(prevMelee.gameObject);

        if (!inventory.AddItem(nextMelee)) return false;

        nextMelee.Attach(equipPosition);
        nextMelee.gameObject.SetActive(false);

        if (wasEquipped)
        {
            currentEquipped = nextMelee;
            nextMelee.OnEquip(this);
            UpdateUpperBodyWeight();
        }

        return true;
    }

    private bool GetSack(Sack nextSack)
    {
        if (nextSack == null) return false;

        Sack prevSack = null;

        foreach (EquippableItem item in inventory.Items)
        {
            if (item is Sack sack && sack.ItemName == nextSack.ItemName)
            {
                prevSack = sack;
                break;
            }
        }

        if (prevSack == null)
        {
            if (!inventory.AddItem(nextSack)) return false;

            nextSack.Attach(equipPosition);
            nextSack.gameObject.SetActive(false);
            return true;
        }

        if (nextSack.Level <= prevSack.Level) return false;

        bool wasEquipped = (currentEquipped == prevSack);

        inventory.RemoveItem(prevSack);

        if (wasEquipped)
        {
            prevSack.OnUnequip(this);
            currentEquipped = null;
            currentSack = null;
        }

        if (!inventory.AddItem(nextSack)) return false;

        nextSack.Attach(equipPosition);
        nextSack.gameObject.SetActive(false);

        prevSack.MoveItems(nextSack);

        Destroy(prevSack.gameObject);

        if (wasEquipped)
        {
            currentEquipped = nextSack;
            nextSack.OnEquip(this);
            UpdateUpperBodyWeight();
        }

        return true;
    }

    public void DropItem()
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

    public bool GetSackItem(Item item)
    {
        if (currentSack == null) return false;
        return currentSack.AddItem(item);
    }

    public void DropSackItem()
    {
        if (currentSack == null) return;

        Item item = currentSack.DropItem();
        if (item == null) return;

        item.transform.position = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
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