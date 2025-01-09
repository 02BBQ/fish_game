using UnityEngine;

[CreateAssetMenu(fileName = "BoatDataSO", menuName = "SO/BoatDataSO")]
public class BoatDataSO : ScriptableObject
{
    public float boatMass;
    public float boatSpeed;
    public float boatWeight;
    public float boatTurn;
    public float boatDamp;
}
