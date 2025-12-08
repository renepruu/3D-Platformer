using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyPersistentBossIfExists();

            SceneManager.LoadScene("GameOverScene"); 
        }
    }

    private void DestroyPersistentBossIfExists()
    {
        const string bossName = "The Boss@Idle(1)";
        var boss = GameObject.Find(bossName);
        if (boss != null)
        {
            Debug.Log($"[FinishLine] Destroying persistent boss '{bossName}' to avoid duplicate cameras.");
            Destroy(boss);
        }
    }
}
