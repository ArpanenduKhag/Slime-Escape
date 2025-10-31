using UnityEngine;

public class PlatformUIController : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }
}
