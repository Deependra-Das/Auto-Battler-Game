using System;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public struct VideoInstructionEntry
{
    public int id;
    public string title;
    public VideoClip videoClip;
    public Sprite videoThumbnail;

    [TextArea(5, 10)]
    public string instruction;
}
