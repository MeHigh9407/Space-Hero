using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip Death;
    [SerializeField] AudioClip Success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem CrashParticles;
    [SerializeField] ParticleSystem SuccessParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;  //AudioSource -this is called TYPE | audioSource -this is called VARIABLE
    
   bool isTransitioning = false;

    bool colisionsDisabled = false;
    public static int sceneCountInBuildSettings;

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        //the debug keys will work if the Developement Build chekcmark is on(Build Menu)
        if (Debug.isDebugBuild)
        { 
        DebugKeys();
        }
    }

    //Here, debug keys used to make testing easyer.
    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            //disable collision
            colisionsDisabled = !colisionsDisabled; // this is a toggle( if collisions are dsabled it enables them and viceversa)
        }

    }

    void OnCollisionEnter(Collision collision) {

        if (isTransitioning || colisionsDisabled) { return; }  //ignore colisions when dead, also stops here due to the "return".

        switch (collision.gameObject.tag) {
            case "Friendly":
                //do nothing
                 break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(Success);
        SuccessParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //parameterise time
    }

    private void StartDeathSequence()
    {
        print("lovit");
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        CrashParticles.Play();
        //This added by me, so the engine particles stop when the rocket crashes
        mainEngineParticles.Stop();
        Invoke("LoadFirstLevel", levelLoadDelay); //parameterise time
    }

    
    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    private void LoadFirstLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex - 1;
        if (nextSceneIndex == -1)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        //float rotationThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space)) //can trust while rotating
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying) //so it does not layer the sound on top of each other
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        
    }
    ///TODO Fix frezer constrait here as it unticks when in play mode
    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
           transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

       // rigidBody.freezeRotation = true; //take manual control of rotation
       // rigidBody.freezeRotation = false; //resume phisics control of rotation
    }

    
}
