using UnityEngine;

public class DataTableValidator
{
    public bool Validate()
    {
        bool isValid = true;

        isValid &= ValidateSackTable();

        return isValid;
    }

    private bool ValidateSackTable()
    {
        SackData oldSack = null;
        SackData goodSack = null;
        SackData giantSack = null;

        foreach (SackData sack in DataManager.Instance.SackTable.All.Values)
        {
            if (sack.Level == SackLevel.OLD)
            {
                oldSack = sack;
            }
            else if (sack.Level == SackLevel.GOOD)
            {
                goodSack = sack;
            }
            else if (sack.Level == SackLevel.GIANT)
            {
                giantSack = sack;
            }
        }

        if (oldSack == null || goodSack == null || giantSack == null)
        {
            Debug.LogError("Sack data is missing.");
            return false;
        }

        if (oldSack.Capacity > goodSack.Capacity || goodSack.Capacity > giantSack.Capacity)
        {
            Debug.LogError($"Sack Capacity rule is invalid, {oldSack.Capacity}, {goodSack.Capacity}, {giantSack.Capacity}");
            return false;
        }

        return true;
    }
}
