// using UnityEngine;

// public class ThrowMechanic : MonoBehaviour
// {
//     private Vector2 startInputPos;
//     private Vector2 endInputPos;
//     private float startTime;
//     private float endTime;
//     private Rigidbody rb;

//     [SerializeField]
//     private float forceMultiplier = 0.002f; // Controls throw strength
//     [SerializeField]
//     private float maxDistance = 15f; // Max distance allowed before ball resets
//     [SerializeField]
//     private float respawnTime = 5f; // Time before auto-respawn

//     [SerializeField]
//     private AudioClip collisionSound; // Sound to play on collision
//     private AudioSource audioSource; // Audio source to play the sound

//     private Vector3 initialPosition;
//     private Quaternion initialRotation;
//     private Vector3 initialScale;
//     private bool isThrown = false;
//     private float respawnTimer;

//     private GameSessionManager gameSessionManager;
//     private ScoreManager scoreManager;
//     private int throwCount = 0;
//     [SerializeField]
//     private int maxThrows = 10; // Maximum number of throws before ending the game

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.useGravity = false;

//         initialPosition = transform.position;
//         initialRotation = transform.rotation;
//         initialScale = transform.localScale;

//         audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
//         gameSessionManager = FindObjectOfType<GameSessionManager>();
//         scoreManager = FindObjectOfType<ScoreManager>();
//     }

//     private void Update()
//     {
//         // Check for touch input on mobile
//         if (Input.touchCount > 0 && !isThrown)
//         {
//             Touch touch = Input.GetTouch(0);

//             if (touch.phase == TouchPhase.Began)
//             {
//                 startInputPos = touch.position;
//                 startTime = Time.time;
//             }
//             else if (touch.phase == TouchPhase.Ended)
//             {
//                 endInputPos = touch.position;
//                 endTime = Time.time;
//                 ThrowBall();
//             }
//         }
//         // Check for mouse input on PC
//         else if (Input.GetMouseButtonDown(0) && !isThrown)
//         {
//             startInputPos = Input.mousePosition;
//             startTime = Time.time;
//         }
//         else if (Input.GetMouseButtonUp(0) && !isThrown)
//         {
//             endInputPos = Input.mousePosition;
//             endTime = Time.time;
//             ThrowBall();
//         }

//         // Start respawn timer once the ball is thrown
//         if (isThrown)
//         {
//             respawnTimer += Time.deltaTime;
//             if (respawnTimer >= respawnTime)
//             {
//                 ResetBall();
//             }
//         }
//     }

//     private void ThrowBall()
//     {
//         Vector2 flickDirection = (endInputPos - startInputPos).normalized;
//         float flickSpeed = (endInputPos - startInputPos).magnitude / (endTime - startTime);

//         // Convert Vector2 direction to a 3D direction for force application
//         Vector3 forceDirection = new Vector3(flickDirection.x, flickDirection.y, 1).normalized;
//         float appliedForce = flickSpeed * forceMultiplier;

//         rb.useGravity = true;
//         rb.AddForce(forceDirection * appliedForce, ForceMode.Impulse);

//         isThrown = true;
//         respawnTimer = 0f; // Reset the timer when ball is thrown
//         throwCount++;
//     }

//     private void LateUpdate()
//     {
//         // Keep the ball from rotating after it's thrown
//         transform.rotation = initialRotation;

//         // Reset ball if it exceeds max distance
//         if (isThrown && Vector3.Distance(transform.position, initialPosition) > maxDistance)
//         {
//             ResetBall();
//         }
//     }

//     private void ResetBall()
//     {
//         // Reset ball to its initial position and state
//         transform.position = initialPosition;
//         transform.rotation = initialRotation;
//         transform.localScale = initialScale;

//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         rb.useGravity = false;
//         isThrown = false;

//         respawnTimer = 0f; // Reset the timer upon respawn

//         // Check if the game should end
//         if (ShouldEndGame())
//         {
//             EndGame();
//         }
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         // Check if the ball collides with any surface and play the collision sound
//         if (audioSource != null && collisionSound != null)
//         {
//             audioSource.PlayOneShot(collisionSound); // Play the collision sound
//         }
//     }

//     private bool ShouldEndGame()
//     {
//         return throwCount >= maxThrows;
//     }

//     private void EndGame()
// {
//     if (gameSessionManager != null)
//     {
//         int finalScore = scoreManager != null ? scoreManager.GetLocalScore() : throwCount;
//         gameSessionManager.EndGame(throwCount, finalScore);
//     }
//     else
//     {
//         Debug.LogWarning("GameSessionManager not found. Unable to end game.");
//     }

//     // Additional end game logic...
// }
// }


// FILEPATH: c:/Users/akram/GAMES_UNITY/Basketball3D/Assets/Scripts/ThrowMechanic.cs

using UnityEngine;

public class ThrowMechanic : MonoBehaviour
{
    private Vector2 startInputPos;
    private Vector2 endInputPos;
    private float startTime;
    private float endTime;
    private Rigidbody rb;

    [SerializeField]
    private float forceMultiplier = 0.002f; // Controls throw strength
    [SerializeField]
    private float rotationMultiplier = 0.5f; // Controls rotation speed after collision
    [SerializeField]
    private float maxDistance = 15f; // Max distance allowed before ball resets
    [SerializeField]
    private float respawnTime = 5f; // Time before auto-respawn

    [SerializeField]
    private AudioClip collisionSound; // Sound to play on collision
    private AudioSource audioSource; // Audio source to play the sound

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isThrown = false;
    private float respawnTimer;

    private GameSessionManager gameSessionManager;
    private ScoreManager scoreManager;
    private int throwCount = 0;
    [SerializeField]
    private int maxThrows = 10; // Maximum number of throws before ending the game

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        gameSessionManager = FindObjectOfType<GameSessionManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Update()
    {
        // Check for touch input on mobile
        if (Input.touchCount > 0 && !isThrown)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startInputPos = touch.position;
                startTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endInputPos = touch.position;
                endTime = Time.time;
                ThrowBall();
            }
        }
        // Check for mouse input on PC
        else if (Input.GetMouseButtonDown(0) && !isThrown)
        {
            startInputPos = Input.mousePosition;
            startTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0) && !isThrown)
        {
            endInputPos = Input.mousePosition;
            endTime = Time.time;
            ThrowBall();
        }

        // Start respawn timer once the ball is thrown
        if (isThrown)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                ResetBall();
            }
        }
    }

    private void ThrowBall()
    {
        Vector2 flickDirection = (endInputPos - startInputPos).normalized;
        float flickSpeed = (endInputPos - startInputPos).magnitude / (endTime - startTime);

        // Convert Vector2 direction to a 3D direction for force application
        Vector3 forceDirection = new Vector3(flickDirection.x, flickDirection.y, 1).normalized;
        float appliedForce = flickSpeed * forceMultiplier;

        rb.useGravity = true;
        rb.AddForce(forceDirection * appliedForce, ForceMode.Impulse);

        isThrown = true;
        respawnTimer = 0f; // Reset the timer when ball is thrown
        throwCount++;
    }

    private void LateUpdate()
    {
        // Reset ball if it exceeds max distance
        if (isThrown && Vector3.Distance(transform.position, initialPosition) > maxDistance)
        {
            ResetBall();
        }
    }

    private void ResetBall()
    {
        // Reset ball to its initial position and state
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        isThrown = false;

        respawnTimer = 0f; // Reset the timer upon respawn

        // Check if the game should end
        if (ShouldEndGame())
        {
            EndGame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the ball collides with any surface and play the collision sound
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound); // Play the collision sound
        }

        // Apply rotation based on collision
        if (isThrown)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 collisionNormal = contact.normal;
            Vector3 ballVelocity = rb.linearVelocity;

            // Calculate rotation axis perpendicular to both the collision normal and ball velocity
            Vector3 rotationAxis = Vector3.Cross(collisionNormal, ballVelocity).normalized;

            // Calculate rotation force based on the angle between collision normal and velocity
            float angle = Vector3.Angle(collisionNormal, ballVelocity);
            float rotationForce = angle * rotationMultiplier;

            // Apply torque
            rb.AddTorque(rotationAxis * rotationForce, ForceMode.Impulse);
        }
    }

    private bool ShouldEndGame()
    {
        return throwCount >= maxThrows;
    }

    private void EndGame()
    {
        if (gameSessionManager != null)
        {
            int finalScore = scoreManager != null ? scoreManager.GetLocalScore() : throwCount;
            gameSessionManager.EndGame(throwCount, finalScore);
        }
        else
        {
            Debug.LogWarning("GameSessionManager not found. Unable to end game.");
        }

        // Additional end game logic...
    }
}
