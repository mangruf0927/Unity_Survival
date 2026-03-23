using TMPro;
using UnityEngine;

public class AmmoCount : MonoBehaviour, IObserver
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private RangedWeapon rangedWeapon;
    
    public void SetWeapon(RangedWeapon weapon)
    {
        if(rangedWeapon != null) rangedWeapon.RemoveObserver(this);
        rangedWeapon = weapon;

        if(rangedWeapon != null)
        {
            rangedWeapon.AddObserver(this);
            Notify();
            gameObject.SetActive(true);
        }
        else
        {
            ammoText.text = "";
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if(rangedWeapon != null) rangedWeapon.RemoveObserver(this);
    }

    public void Notify()
    {
        ammoText.text = $"{rangedWeapon.CurrentAmmo} / {rangedWeapon.TotalAmmo}";
    }
}
