using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{
    public float jumpForce = 20f;
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody _rigidbody))
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
        }
    }
}
