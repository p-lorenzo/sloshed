using UnityEngine;

public class OnLooseBalance : MonoBehaviour
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
