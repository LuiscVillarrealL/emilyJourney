
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeSO : ScriptableObject
{
    public string UpgradeId;
    public string Description;
    public string DescriptionAfter;
    public int cost;
}
