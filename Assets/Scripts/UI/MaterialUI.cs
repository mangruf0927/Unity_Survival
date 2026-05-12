using TMPro;
using UnityEngine;

public class MaterialUI : MonoBehaviour, IObserver
{
    [SerializeField] private Grinder grinder;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI ironText;

    private void Start()
    {
        grinder.AddObserver(this);
        Notify();
    }

    private void OnDestroy()
    {
        grinder.RemoveObserver(this);
    }

    public void Notify()
    {
        woodText.text = grinder.Wood.ToString();
        ironText.text = grinder.Iron.ToString();
    }
}
