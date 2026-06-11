using System.Collections.Generic;
using UnityEngine;

public class InventoryUnitCardPool
{
    private readonly Queue<InventoryUnitCard> _pool = new();
    private readonly Transform _activeCardContainer;
    private readonly Transform _poolCardContainer;

    public InventoryUnitCardPool(InventoryUnitCard prefab, Transform activeContainer, Transform poolContainer, int poolSize)
    {
        _activeCardContainer = activeContainer;
        _poolCardContainer = poolContainer;

        for (int i = 0; i < poolSize; i++)
        {
            var card = Object.Instantiate(prefab, _poolCardContainer);
            card.gameObject.SetActive(false);
            _pool.Enqueue(card);
        }
    }

    public InventoryUnitCard Get()
    {
        if (_pool.Count == 0)
        {
            Debug.LogError("Inventory pool exhausted!");
            return null;
        }

        var card = _pool.Dequeue();
        card.transform.SetParent(_activeCardContainer, false);
        card.gameObject.SetActive(true);

        return card;
    }

    public void Release(InventoryUnitCard card)
    {
        card.Reset();
        card.transform.SetParent(_poolCardContainer, false);
        card.gameObject.SetActive(false);

        _pool.Enqueue(card);
    }

    public void ReleaseAll(List<InventoryUnitCard> activeCards)
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            Release(activeCards[i]);
        }
    }
}