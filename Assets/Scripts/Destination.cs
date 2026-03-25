using UnityEngine;
using UnityEngine.SceneManagement;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private float reachDistance = 0.25f;

    private bool isLoadingNextLevel;
    private Player player;
    private float nextDebugLogTime;

    private void Start()
    {
        player = Object.FindFirstObjectByType<Player>();
        Debug.Log($"Destination ready on {name}. Target scene: '{sceneName}'", this);
        if (player == null)
        {
            Debug.LogWarning("Destination could not find a Player at startup.", this);
        }
        else
        {
            Debug.Log($"Destination found player '{player.name}' at {player.transform.position}.", player);
        }
    }

    private void Update()
    {
        if (isLoadingNextLevel)
        {
            return;
        }

        if (player == null)
        {
            player = Object.FindFirstObjectByType<Player>();
            if (player != null)
            {
                Debug.Log($"Destination found player '{player.name}' during Update at {player.transform.position}.", player);
            }
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Destination has no scene name assigned.", this);
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (Time.time >= nextDebugLogTime)
        {
            nextDebugLogTime = Time.time + 1f;
            Debug.Log($"Destination distance check. Player: {player.transform.position}, Goal: {transform.position}, Distance: {distanceToPlayer}, Reach Distance: {reachDistance}", this);
        }

        if (distanceToPlayer > reachDistance)
        {
            return;
        }

        Debug.Log($"Destination loading scene '{sceneName}' after player {player.name} entered.", this);
        isLoadingNextLevel = true;
        SceneManager.LoadScene(sceneName);
    }
}
