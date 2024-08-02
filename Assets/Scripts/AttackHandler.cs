using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class AttackHandler : NetworkBehaviour
{
    [Networked] public TickTimer picker { get; set; }

    public void Init()
    {
        Object.RequestStateAuthority();
        Debug.Log(Object.HasStateAuthority);
        picker = TickTimer.CreateFromSeconds(Runner, 5f);

        transform.GetComponent<NetworkRigidbody2D>().Rigidbody.AddForce(Vector2.right * 100f, ForceMode2D.Impulse);
    }

    public override void FixedUpdateNetwork()
    {
        if (picker.Expired(Runner))
        {
            Object.ReleaseStateAuthority();
            Runner.Despawn(Object);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);

        if (Object.HasStateAuthority)
        {

            Object.ReleaseStateAuthority();

            Runner.Despawn(Object);
        }
    }
}
