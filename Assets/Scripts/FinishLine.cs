using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ensure persistent boss is removed before switching scenes
            DestroyPersistentBossIfExists();

            SceneManager.LoadScene("GameOverScene"); 
        }
    }

    private void DestroyPersistentBossIfExists()
    {
        // exact name used in the scene hierarchy when the boss was instantiated
        const string bossName = "The Boss@Idle(1)";
        var boss = GameObject.Find(bossName);
        if (boss != null)
        {
            Debug.Log($"[FinishLine] Destroying persistent boss '{bossName}' to avoid duplicate cameras.");
            Destroy(boss);
        }
    }
}
