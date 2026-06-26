using TMPro;
using UnityEngine;

public class SackItemCount : MonoBehaviour, IObserver
{
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Sack sack;

    private void OnEnable()
    {
        sack.AddObserver(this);
        Notify();
    }

    private void OnDisable()
    {
        sack.RemoveObserver(this);
    }

    public void Notify()
    {
        if (sack == null || count == null) return;

        count.text = $"{sack.Count} / {sack.Capacity}";
    }
}
