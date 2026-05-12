using TMPro;
using UnityEngine;

public class MaterialUI : MonoBehaviour, IObserver
{
    [SerializeField] private WorkTableInventory inventory;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI ironText;

    private void Start()
    {
        inventory.AddObserver(this);
        Notify();
    }

    private void OnDestroy()
    {
        inventory.RemoveObserver(this);
    }

    public void Notify()
    {
        woodText.text = inventory.Wood.ToString();
        ironText.text = inventory.Iron.ToString();
    }
}
