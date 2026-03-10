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
    public bool IsGround() { return isGround;}
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
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target,Time.fixedDeltaTime * playerStat.rotateSpeed));
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
        if(item is MeleeWeapon nextMelee) return GetMeleeWeapon(nextMelee);

        if(!inventory.AddItem(item)) return false;

        item.Attach(equipPosition);
        item.gameObject.SetActive(false);
        return true;
    }

    private bool GetMeleeWeapon(MeleeWeapon nextMelee)
    {
        if (nextMelee == null) return false;

        MeleeWeapon currentMelee = null;

        foreach (EquippableItem item in inventory.Items)
        {
            if (item is MeleeWeapon melee && melee.ItemName == nextMelee.ItemName)
            {
                currentMelee = melee;
                break;
            }
        }

        if (currentMelee == null)
        {
            if (!inventory.AddItem(nextMelee)) return false;

            nextMelee.Attach(equipPosition);
            nextMelee.gameObject.SetActive(false);
            return true;
        }

        if (nextMelee.Level <= currentMelee.Level) return false;

        bool wasEquipped = currentEquipped == currentMelee;

        inventory.RemoveItem(currentMelee);

        if (wasEquipped)
        {
            currentMelee.OnUnequip(this);
            currentEquipped = null;
            currentWeapon = null;
        }

        Destroy(currentMelee.gameObject);

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
        if(nextItem == null) return;

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
        if(currentSack == null) return;
        currentSack.DropItem();
    }

    private void UpdateUpperBodyWeight()
    {
        animator.SetLayerWeight(1, currentEquipped == null ? 0f : 1f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGround = true;
    }
}
