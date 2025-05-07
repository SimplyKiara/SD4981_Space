using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public int res_h;
    public int res_w;
    void Start()
    {
        // Optionally clear PlayerPrefs
        // PlayerPrefs.DeleteAll();

        // Set the desired resolution
        Screen.SetResolution(res_h, res_w, FullScreenMode.Windowed);
    }
}
