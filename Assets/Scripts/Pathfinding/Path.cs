using UnityEngine;

public class Path
{
    public Node source;
    public Node destination;
    private float _weight;

    public Path(Node from, Node to, float weight)
    {
        this.source = from;
        this.destination = to;
        this._weight = weight;
    }

    public float GetWeight()
    {
        if (destination.IsOccupied)
            return Mathf.Infinity;

        return _weight;
    }
}
