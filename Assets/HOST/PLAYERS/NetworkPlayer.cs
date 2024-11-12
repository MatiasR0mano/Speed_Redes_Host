using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LocalInputs))]
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }
    public LocalInputs Inputs { get; private set; }

    public Transform pos;
    public NetworkRigidbody2D _net_rb2D;
    public bool controlEnabled = true, ganador;
    public JumpState jumpState = JumpState.Grounded;
    Vector3 puntero;
    public Camera camara;
    public GameObject aim, verdadero_aim;
    [Header("Move2d")]
    public SpriteRenderer spriteRenderer;
    public float move_horizontal;
    public float _speed, _smooth_move;
    public Vector3 velocity = Vector3.zero;
    [Header("Jump2D")]
    public Vector2 targetVelocity, velocity2;
    protected Vector2 groundNormal;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    public bool Jump = false, Grounded = false, stopJump, double_jump, stop_double_jump;
    public float minMoveDistance, shellRadius, minGroundNormalY, gravityModifier,
        jumpTakeOffSpeed, jumpModifier, jumpDeceleration;

    [Header("WallJump2D")]
    public LayerMask Wall;
    [SerializeField] Transform controll_wall;
    [SerializeField] Vector3 shell_wall;
    public bool _inWall, _slide, _jump_Wall, takedamage;
    [SerializeField]
    float _strengthJump_WallX, _strengthJump_WallY, _timeJump_Wall, _strengthDamage_Y, _strengthDamage_X;
    public float cooldown_damage_max;

    public Skills skill;

    public Vector2 damage_rebote;
    public float time_enable;
    public float cooldown_damage;

    Iinteract interact;
    public bool interactuo;
    public LayerMask interact_mask;

    public override void Spawned()
    {
        //Gamemanager.instance.AddToList(this);
        if (HasInputAuthority)
        {
            if (Camera.main.TryGetComponent(out Camera_Follow follow)) follow.CameraCine(this);
            Cambio_color(Runner.LocalPlayer);
        }
        camara = Camera.main;
        Inputs = GetComponent<LocalInputs>();

        if (Object.HasInputAuthority)
        {
            Local = this;
            Inputs.enabled = true;
        }
        else
        {
            Inputs.enabled = false;
        }
    }

    private void Update()
    {
        if (controlEnabled)
        {
            cooldown_damage = cooldown_damage <= cooldown_damage_max ? cooldown_damage += Runner.DeltaTime : cooldown_damage;
            targetVelocity = Vector2.zero;
            move_desde_aca();
            Confirmar();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (controlEnabled)
        {
            GravityPlayer();
            //CheckJumpWall2D();
        }
    }
    public void Move2D(float move)
    {
        if (controlEnabled)
        {
            Vector3 _moveDirection = transform.right * (velocity2.x = move * _speed * Runner.DeltaTime);
            _net_rb2D.Rigidbody.velocity = Vector3.SmoothDamp(_net_rb2D.Rigidbody.position, _moveDirection, ref velocity, _smooth_move);
            if (move > 0.01f)
            {
                spriteRenderer.flipX = false;
                controll_wall.position = new Vector2(transform.position.x + 1f, transform.position.y);
            }
            else if (move < -0.01f)
            {
                spriteRenderer.flipX = true;
                controll_wall.position = new Vector2(transform.position.x - 1f, transform.position.y);
            }
            else velocity = Vector3.zero;
        }
    }

    public void Jump2D(bool input)
    {
        Jump = input;
        if (controlEnabled && !takedamage)
        {
            if (jumpState == JumpState.Grounded && Jump)
            {
                jumpState = JumpState.PrepareToJump;
            }
            else if (input) stopJump = true;

            if (_inWall && _slide)
            {
                Debug.Log("muro");
                JumpWall2D(input);
            }

        }
    }

    void PerformMovement(Vector2 move, bool yMovement)
    {
        var distance = move.magnitude;
        if (distance > minMoveDistance)
        {
            var count = _net_rb2D.Rigidbody.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            for (var i = 0; i < count; i++)
            {
                var currentNormal = hitBuffer[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    Grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                if (Grounded)
                {
                    var projection = Vector2.Dot(velocity2, currentNormal);
                    if (projection < 0) velocity2 -= projection * currentNormal;
                }
                else
                {
                    velocity2.x *= 0;
                    velocity2.y = Mathf.Min(velocity2.y, 0);
                }
                var modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        _net_rb2D.Rigidbody.position += move.normalized * distance;
    }

    public void UpdateJumpState(bool input)
    {
        Jump = input;
        Jump = false;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                Jump = true;
                break;
            case JumpState.Jumping:
                if (!Grounded) jumpState = JumpState.InFlight;
                break;
            case JumpState.InFlight:
                if (Grounded) jumpState = JumpState.Landed;
                break;
            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                break;
        }
    }

    public void GravityPlayer()
    {
        if (_inWall)
        {
            velocity2 += Runner.DeltaTime * Physics2D.gravity;

        }
        else
        {
            velocity2 += Runner.DeltaTime * Physics2D.gravity * 10;
            velocity2.x = targetVelocity.x;

            Grounded = false;

            var deltaPosition = velocity2 * Runner.DeltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);
        }
    }

    public void Jump_model(bool input, bool input_double)
    {
        Jump = input;
        double_jump = input_double;
        if (Jump && Grounded)
        {
            if (!_inWall) velocity2.y = jumpTakeOffSpeed * jumpModifier;
            else velocity2.y = 0;
            stop_double_jump = false;
            Jump = false;
        }
        if (double_jump && !Grounded && !stop_double_jump)
        {
            if (!_inWall) velocity2.y += (jumpTakeOffSpeed * 1.2f) * (jumpModifier * 1.2f);
            else velocity2.y = 0;
            double_jump = false;
            stop_double_jump = true;
        }
        else if (stopJump)
        {
            stopJump = false;
            if (velocity2.y > 0) velocity2.y *= jumpDeceleration;
        }
    }
    public void move_desde_aca()
    {
        targetVelocity = velocity2 * _speed;
    }

    public void CheckJumpWall2D(float input)
    {
        if (!Grounded && _inWall)
        {
            _slide = true;
        }
        else
        {
            _slide = false;
        }
        _inWall = Physics2D.OverlapBox(controll_wall.position, shell_wall, 0f, Wall);
    }

    public void JumpWall2D(bool input)
    {
        _inWall = false;

        if (spriteRenderer.flipX)
        {
            _net_rb2D.Rigidbody.velocity = new Vector2(_strengthJump_WallX * -velocity2.x, velocity2.y = _strengthJump_WallY);
            Debug.Log("jumpWall2D");

        }
        else
        {
            _net_rb2D.Rigidbody.velocity = new Vector2(_strengthJump_WallX * velocity2.x, velocity2.y = _strengthJump_WallY);
        }
    }

    public void Rotation_AIM(GameObject aim, Vector3 mouse)
    {
        puntero = camara.ScreenToWorldPoint(mouse);
        float angleRad = Mathf.Atan2(puntero.y - aim.transform.position.y, puntero.x - aim.transform.position.x);
        float angleGrad = (180 / Mathf.PI) * angleRad - 90;
        aim.transform.rotation = Quaternion.Euler(0, 0, angleGrad);
    }

    public void Rebote()
    {
        if (!spriteRenderer.flipX)
        {
            _net_rb2D.Rigidbody.velocity = new Vector2(0, velocity2.y = damage_rebote.y);
        }
        else
        {
            _net_rb2D.Rigidbody.velocity = new Vector2(0, velocity2.y = damage_rebote.y);
        }
    }

    public IEnumerator Lose_controll()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        _speed = 0;
        velocity2.y = 0;
        targetVelocity.y = 0;
        takedamage = true;
        Debug.Log("control");
        yield return new WaitForSeconds(time_enable);
        _speed = 50;
        takedamage = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    public void TakeDamage()
    {
        if (cooldown_damage >= cooldown_damage_max)
        {
            StartCoroutine(Lose_controll());
            Rebote();
            cooldown_damage = 0;
        }
    }

    public void Confirmar()
    {
        interactuo = Physics2D.OverlapBox(controll_wall.position, shell_wall, 0f, interact_mask);
        switch (interactuo)
        {
            case true:
                Collider2D[] hit = Physics2D.OverlapBoxAll(controll_wall.position, shell_wall, 0f, interact_mask);
                foreach (Collider2D hitcollider in hit)
                {
                    interact = hitcollider.GetComponent<Iinteract>();
                    interact.RPC_interact(Runner.LocalPlayer, this);
                }
                break;
        }
    }

    public void Cambio_color(PlayerRef player)
    {
        if (player == Runner.LocalPlayer) spriteRenderer.color = Color.cyan;
        else spriteRenderer.color = Color.red;
    }

    public void Habilidad_Skill()
    {
        Runner.Spawn(skill, verdadero_aim.transform.position, verdadero_aim.transform.rotation);
        skill.Habilidad(this);
        skill = null;
        aim.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controll_wall.position, shell_wall);
    }

}
