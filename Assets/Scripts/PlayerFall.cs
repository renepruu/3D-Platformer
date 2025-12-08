using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerFallReset : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -2f;

    // Kui -1, laetakse aktiivne stseen uuesti
    [SerializeField] private int sceneIndexToLoad = -1;

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            DestroyPersistentBossIfExists();
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        int index = sceneIndexToLoad;

        if (index == -1)
            index = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(index);
    }


    private void DestroyPersistentBossIfExists()
    {
        const string bossName = "The Boss@Idle(1)";
        var boss = GameObject.Find(bossName);
        if (boss != null)
        {
            Debug.Log($"[PlayerFallReset] Destroying persistent boss '{bossName}' before scene reload.");
            Destroy(boss);
        }
    }
}
