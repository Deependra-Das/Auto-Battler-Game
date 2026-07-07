using System.Collections.Generic;
using UnityEngine;

public class VideoInstructionService
{
    private Dictionary<int, VideoInstructionEntry> _videoInstructionDictionary;

    public VideoInstructionService(VideoInstructionScriptableObjectScript videoInstruction_SO)
    {
        if (videoInstruction_SO != null)
        {
            _videoInstructionDictionary = new Dictionary<int, VideoInstructionEntry>();

            foreach (var entry in videoInstruction_SO.videoInstructionList)
            {
                _videoInstructionDictionary.Add(entry.id, entry);
            }
        }
    }

    public int Count => _videoInstructionDictionary.Count;

    public VideoInstructionEntry GetVideoInstructionEntry(int index)
    {
        return _videoInstructionDictionary[index];
    }
}
