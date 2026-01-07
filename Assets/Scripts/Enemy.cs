using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float chaseRange = 20f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool destroyOnHit = false;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform _player;
    private Rigidbody _rb;
    private float _lastAttackTime = -999f;

    public AudioSource _audioSource;

    private void Start()
    {
        GameObject playerObj = null;
        try { playerObj = GameObject.FindWithTag("player"); } catch { }
        if (playerObj == null)
        {
            try { playerObj = GameObject.FindWithTag("Player"); } catch { }
        }

        _player = playerObj.transform;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rb != null) return;

        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > chaseRange) return;

        if (dist > stopDistance)
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            Vector3 targetPos = transform.position + direction * moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        TryMeleeAttack(); 
    }

    private void FixedUpdate()
    {
        if (_rb == null || _player == null) return;

        float dist = Vector3.Distance(_rb.position, _player.position);
        if (dist > chaseRange) return;

        if (dist > stopDistance)
        {
            Vector3 direction = (_player.position - _rb.position).normalized;
            Vector3 newPos = _rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(newPos);

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
            }
        }

        TryMeleeAttack();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDealDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other.gameObject);
    }

    private void TryMeleeAttack()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= attackRange && Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;

            TryDealDamage(_player.gameObject);
        }
    }

    private void TryDealDamage(GameObject target)
    {
        if (_player == null || target == null) return;

        Transform t = target.transform;
        bool isPlayerHit = (t == _player) || t.IsChildOf(_player);
        if (!isPlayerHit) return;

        GameObject playerGO = _player.gameObject;

        bool handled = false;

        var candidateNames = new[] { "Player" };
        foreach (var name in candidateNames)
        {
            var comp = playerGO.GetComponent(name) as Component;
            if (comp == null) continue;
            var method = comp.GetType().GetMethod("TakeDamage", new[] { typeof(float) });
            if (method != null)
            {
                method.Invoke(comp, new object[] { damage });
                handled = true;
                break;
            }
            method = comp.GetType().GetMethod("ApplyDamage", new[] { typeof(float) });
            if (method != null)
            {
                method.Invoke(comp, new object[] { damage });
                handled = true;
                break;
            }
        }

        if (!handled)
        {
            var monos = playerGO.GetComponents<MonoBehaviour>();
            foreach (var mb in monos)
            {
                if (mb == null) continue;
                var method = mb.GetType().GetMethod("TakeDamage", new[] { typeof(float) });
                if (method != null)
                {
                    method.Invoke(mb, new object[] { damage });
                    handled = true;
                    break;
                }
                method = mb.GetType().GetMethod("ApplyDamage", new[] { typeof(float) });
                if (method != null)
                {
                    method.Invoke(mb, new object[] { damage });
                    handled = true;
                    break;
                }
            }
        }

        if (!handled)
        {
            playerGO.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = new Color(1f, 0.5f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}