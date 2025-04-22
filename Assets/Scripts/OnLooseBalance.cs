using UnityEngine;

public class OnLooseBalance : MonoBehaviour
{
    public void Fallen()
    {
        GameManager.instance.Fallen();
    }
}
