using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInstance : MonoBehaviour
{
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
            if (SceneManager.GetActiveScene().name == "TabletScene")
            {
                foreach (Transform child2 in child.transform)
                {
                    if (child2.gameObject.name != "GroupName")
                    {
                        child2.gameObject.SetActive(isActive);
                        SetVisibilityAndInteractivity(child2.gameObject, isActive);
                    }
                }
            }
            else
            {
                child.gameObject.SetActive(isActive);
                foreach (Transform child2 in child.transform)
                {
                    if (child2.gameObject.name != "GroupName")
                    {
                        SetVisibilityAndInteractivity(child.gameObject, isActive);
                    }
                }
            }
        }
    }

    void SetVisibilityAndInteractivity(GameObject obj, bool visible)
    {
        // Handle UI (CanvasGroup)
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;

        // Handle 3D/2D objects
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = visible;

        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) collider.enabled = visible;
    }

}
