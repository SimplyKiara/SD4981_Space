using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInstance : MonoBehaviour
{
    private static PersistentInstance instance;
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
        if (SceneManager.GetActiveScene().name == "ClientMenu")
        {
            Debug.Log(SceneManager.GetActiveScene().name + "");
            SetAllChildrenActive(gameObject, true);
        }
        else if (SceneManager.GetActiveScene().name != "ClientMenu" && gameObject.activeInHierarchy)
        {
            Debug.Log(SceneManager.GetActiveScene().name + "");
            SetAllChildrenActive(gameObject, false);
        }
    }

    void SetAllChildrenActive(GameObject parent, bool isActive)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

}
