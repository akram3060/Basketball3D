using UnityEngine;

public class BasketCollision : MonoBehaviour
{
    public ScoreManager scoreManager;
    private bool enteredFromTop = false; // Tracks if ball entered from top

    [SerializeField]
    private AudioClip scoreSound; // Sound to play when score is updated
    private AudioSource audioSource; // AudioSource to play sounds

    private void Start()
    {
        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is the basketball and entered from above
        if (other.CompareTag("Basketball") && other.transform.position.y > transform.position.y)
        {
            enteredFromTop = true; // Ball entered from the top
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object is the basketball and exits below
        if (enteredFromTop && other.CompareTag("Basketball") && other.transform.position.y < transform.position.y)
        {
            scoreManager.AddScore(1); // Update score
            
            // Play score sound when a point is scored
            if (audioSource != null && scoreSound != null)
            {
                audioSource.PlayOneShot(scoreSound);
            }

            enteredFromTop = false; // Reset for the next throw
        }
    }
}
