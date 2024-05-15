using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "LibraWordSO", menuName = "LibraWordSO")]
public class LibraWordSO : ScriptableObject
{
    public VideoClip video;
    public string word;

    public string[] incorrectWords;
}
