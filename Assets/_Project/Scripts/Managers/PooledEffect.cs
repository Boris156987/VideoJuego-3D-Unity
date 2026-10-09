using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;

    private ParticleSystem particles;
    private EffectPool pool;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    public void SetPool(EffectPool owner)
    {
        pool = owner;
    }

    private void OnEnable()
    {
        particles.Clear();
        particles.Play();
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnToPool()
    {
        if (pool != null)
            pool.Return(this);
    }
}
