using UnityEngine;

public class OnLoseBalance : MonoBehaviour
{
    public void Fallen()
    {
        GameManager.instance.Fallen();
    }

    public void OnRegainBalance()
    {
        GameManager.instance.GetUp();
    }
}
