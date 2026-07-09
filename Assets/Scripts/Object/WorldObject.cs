using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [SerializeField] private int itemId;
    [SerializeField] private ObjectType objectType;

    private long instanceId;
    public long InstanceId => instanceId;
    public int ItemId => itemId;
    public ObjectType ObjectType => objectType;

    public void SetInstanceId(long id)
    {
        instanceId = id;
    }

    public virtual ObjectSaveData CreateSaveData()
    {
        Vector3 position = transform.position;
        Vector3 rotation = transform.eulerAngles;

        return new ObjectSaveData
        {
            instanceId = instanceId,
            itemId = itemId,
            objectType = objectType,

            positionX = position.x,
            positionY = position.y,
            positionZ = position.z,

            rotationX = rotation.x,
            rotationY = rotation.y,
            rotationZ = rotation.z,

            isActive = gameObject.activeSelf
        };
    }

    public virtual void LoadSaveData(ObjectSaveData data)
    {
        if (data == null) return;

        Vector3 position = new(data.positionX, data.positionY, data.positionZ);
        Quaternion rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);

        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(data.isActive);
    }
}
