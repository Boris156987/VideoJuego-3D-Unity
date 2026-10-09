using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance { get; private set; }

    [SerializeField] private PooledEffect prefab;
    [SerializeField] private int initialSize = 3;

    private readonly Queue<PooledEffect> available = new Queue<PooledEffect>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
            available.Enqueue(CreateInstance());
    }

    private PooledEffect CreateInstance()
    {
        PooledEffect effect = Instantiate(prefab, transform);
        effect.SetPool(this);
        effect.gameObject.SetActive(false);
        return effect;
    }

    public void Play(Vector3 position)
    {
        PooledEffect effect = available.Count > 0 ? available.Dequeue() : CreateInstance();

        effect.transform.position = position;
        effect.gameObject.SetActive(true);
    }

    public void Return(PooledEffect effect)
    {
        effect.gameObject.SetActive(false);
        available.Enqueue(effect);
    }
}
