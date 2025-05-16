using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInstance : MonoBehaviour
{
    // Load child under this manager when switching scenes
    private static PersistentInstance instance;
    private bool isChildActive;
    void Start()
    {
        isChildActive = true;
    }
    void Awake()
    {
        // Ensure that only one instance of the persistent object exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if ((SceneManager.GetActiveScene().name == "ClientMenu" || SceneManager.GetActiveScene().name == "MainWallScene" || SceneManager.GetActiveScene().name == "ResourcesScene") && !isChildActive)
        {
            Debug.Log(SceneManager.GetActiveScene().name + "");
            SetAllChildrenActive(gameObject, true);
            isChildActive = true;
        }
        else if (!(SceneManager.GetActiveScene().name == "ClientMenu" || SceneManager.GetActiveScene().name == "MainWallScene" || SceneManager.GetActiveScene().name == "ResourcesScene") && isChildActive)
        {
            Debug.Log(SceneManager.GetActiveScene().name + "");
            SetAllChildrenActive(gameObject, false);
            isChildActive = false;
        }
    }

    void SetAllChildrenActive(GameObject parent, bool isActive)
    {
        foreach (Transform child in parent.transform)
        {
            foreach (Transform child2 in child.transform)
            {
                if (child2.gameObject.name != "GroupName")
                    child2.gameObject.SetActive(isActive);
            }
        }
    }
}
