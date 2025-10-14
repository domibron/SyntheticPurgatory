// using System.IO;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class ABCPlayer : MonoBehaviour
{

    [SerializeField]
    AlembicStreamPlayer streamPlayer;

    [SerializeField]
    bool isPlaying = false;

    [SerializeField]
    bool isReversed = false;

    [SerializeField]
    float speed = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // string path = Path.Combine(Application.streamingAssetsPath, fileName);



        streamPlayer = GetComponent<AlembicStreamPlayer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying && !isReversed)
            streamPlayer.CurrentTime += Time.deltaTime * speed;
        else if (isPlaying && isReversed)
            streamPlayer.CurrentTime -= Time.deltaTime * speed;

        if (isReversed)
        {
            if (streamPlayer.CurrentTime <= streamPlayer.StartTime)
            {
                streamPlayer.CurrentTime = streamPlayer.EndTime;
            }
        }
        else
        {
            if (streamPlayer.CurrentTime >= streamPlayer.EndTime)
            {
                streamPlayer.CurrentTime = streamPlayer.StartTime;
            }
        }
    }
}
