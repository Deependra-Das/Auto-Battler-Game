using UnityEngine;

public class Node
{
    public int index;
    public Vector3 position;
    private bool _isOccupied = false;

    public Node(int index, Vector3 position)
    {
        this.index = index;
        this.position = position;
        _isOccupied = false;
    }

    public void SetOccupied(bool value)
    {
        _isOccupied = value;
    }

    public bool IsOccupied => _isOccupied;
}