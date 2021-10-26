
using UnityEngine;

public class SFXLibInit : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.SetNewSFXLibrary(gameObject);
    }
}
