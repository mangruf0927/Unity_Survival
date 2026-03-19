using TMPro;
using UnityEngine;

public class SackItemCount : MonoBehaviour, IObserver
{
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Sack sack;
    
    private void Start()
    {
        sack.AddObserver(this);
        Notify();       
    }

    private void OnDestroy()
    {
        sack.RemoveObserver(this);
    }

    public void Notify()
    {
        count.text = $"{sack.Count} / {sack.Capacity}";
    }
}
