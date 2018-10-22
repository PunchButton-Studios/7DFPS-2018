using UnityEngine;

[RequireComponent(typeof(AudioController))]
public class SpookSoundsHandler : MonoBehaviour
{
    public Transform player;
    public AudioController audioController;
    private float nextSound;
    public float minInterval = 15.0f, maxInterval = 40.0f;
    [Range(0, 1)] public float minVolume = 0.2f;
    [Range(0, 1)] public float maxVolume = 0.5f;

    public float minDistance = 10.0f, maxDistance = 50.0f;

    private void Reset()
    {
        audioController = GetComponent<AudioController>();
    }

    private void Start()
    {
        nextSound = Time.time + Random.Range(minInterval, maxInterval);
    }

    private void Update()
    {
        if (Time.time > nextSound)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * Random.Range(minDistance, maxDistance);
            transform.position = player.position + position;
            audioController.Volume = Random.Range(minVolume, maxVolume);
            audioController.PlayRandom();
            nextSound = Time.time + Random.Range(minInterval, maxInterval);
        }
    }
}