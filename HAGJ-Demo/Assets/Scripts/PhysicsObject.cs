using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    [Header("Physics")]
    public float gravityModifier = 1f;
    public float minGroundNormalY = 0.65f;

    protected Vector2 targetVelocity;
    protected bool facingRight = true;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    public virtual void Awake() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;
        SetGroundedForAnimation(grounded);

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    public virtual void Update() {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() {

    }

    protected virtual void SetGroundedForAnimation(bool groundedForAnimation) {

    }

    void Movement(Vector2 move, bool yMovement) {
        float distance = move.magnitude;
        if (distance > minMoveDistance) {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }
            for (int i = 0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY) {
                    grounded = true;
                    SetGroundedForAnimation(grounded);
                    if (yMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0) {
                    velocity = velocity - projection * currentNormal;
                }

                float modiefiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modiefiedDistance < distance ? modiefiedDistance : distance;
            }
        }
        rb2d.position = rb2d.position + move.normalized * distance;
        if (move.x > 0 && !facingRight) {
            FlipLookDirection();
        }
        else if (move.x < 0 && facingRight) {
            FlipLookDirection();
        }
    }

    private void FlipLookDirection() {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}