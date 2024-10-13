using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Start()
    {
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
