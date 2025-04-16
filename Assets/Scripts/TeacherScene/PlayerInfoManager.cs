using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    public GameObject playerInfoCardPrefab; // Reference to the Player Info Card prefab
    public Transform playerInfoGrid; // Reference to the Grid Layout Group parent
    private string serverUrl = "http://localhost:3000/players"; // Replace with your server URL

    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public string progress;
        // Other player fields if needed
    }

    void Start()
    {
        StartCoroutine(GetPlayerData());
    }

    IEnumerator GetPlayerData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                PlayerData[] players = JsonUtility.FromJson<PlayerData[]>(jsonString);
                foreach (PlayerData player in players)
                {
                    AddPlayerInfoCard(player);
                }
            }
        }
    }

    void AddPlayerInfoCard(PlayerData playerData)
    {
        GameObject newCard = Instantiate(playerInfoCardPrefab, playerInfoGrid);
        newCard.transform.Find("PlayerNameText").GetComponent<Text>().text = playerData.name;
        newCard.transform.Find("PlayerProgressText").GetComponent<Text>().text = playerData.progress;
    }
}
