using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _attackPrefab;
    private NetworkRigidbody2D _rigidbody;
    private Vector3 _directionMove;
    private bool _isAttack;
    private bool _attackRequested;

    private void Awake()
    {
        _rigidbody = GetComponent<NetworkRigidbody2D>();

        //Local = this;

    }
    private void Start()
    {
        if (Object.HasStateAuthority)
        {
            InitializeAuthority();
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");

        }
    }


    private void InitializeAuthority()
    {
        Object.RequestStateAuthority();
    }
    private void Update()
    {
        if (!Object.HasStateAuthority) return;

        _directionMove = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (!_isAttack)
        {
            _isAttack = Input.GetKeyDown(KeyCode.Z);
            Debug.Log("Key down: " + _isAttack);
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        OnMove(2f);

        if (_isAttack)
        {
            OnAttack();
            _isAttack = false;
            //_attackRequested = false;
        }

    }

    [Networked] public NetworkObject attack { get; set; }

    public void OnAttack()
    {
        Debug.Log("Spawn + " + _rigidbody.transform.position);
        attack = Runner.Spawn(_attackPrefab,
         transform.position,
         Quaternion.Euler(Vector3.zero),
         Object.InputAuthority,
         (runner, o) =>
         {
             // Initialize the Ball before synchronizing it
             o.GetComponent<AttackHandler>().Init();
         });

    }

    private void OnMove(float speed)
    {
        _directionMove.Normalize();
        _rigidbody.Rigidbody.velocity = _directionMove * 50 * Runner.DeltaTime;

    }
    [Networked] public float Health { get; set; }
    public void TakeDamage(float damage)
    {
        Health -= damage;
    }


    private void OnCollisio2DEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet")) return;
        if (Object.HasStateAuthority)
        {

            Debug.Log("Bullet collided with " + collision.gameObject.name);
            Vector3 direction = -collision.transform.position + transform.position;
            _rigidbody.Rigidbody.AddForce(direction.normalized * 50f, ForceMode2D.Impulse);

        }

    }


}
