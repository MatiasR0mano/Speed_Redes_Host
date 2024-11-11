using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public enum JumpState
{
    Grounded, PrepareToJump, Jumping, InFlight, Landed
}

public class Player2 : NetworkBehaviour
{
    [SerializeField] NetworkRigidbody2D _net_rb2D;
    public bool controlEnabled = true, ganador;
    public JumpState jumpState = JumpState.Grounded;
    Vector3 puntero;
    public Camera camara;
    public GameObject aim, verdadero_aim;
    public Animator anim;
    [Header("Move2d")]
    public SpriteRenderer spriteRenderer;
    float move_horizontal;
    [SerializeField] float _speed = 450, _smooth_move = .19f;
    Vector3 velocity = Vector3.zero;
    [Header("Jump2D")]
    protected Vector2 targetVelocity, groundNormal, velocity2;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    bool Jump = false, Grounded = false, stopJump;
    public float minMoveDistance = 0.001f, shellRadius = 0.01f, minGroundNormalY = .65f, gravityModifier = 1f,
        jumpTakeOffSpeed = 7, jumpModifier = 1.5f, jumpDeceleration = 0.5f;

    [Header("WallJump2D")]
    public LayerMask Wall;
    [SerializeField] Transform controll_wall;
    [SerializeField] Vector3 shell_wall;
    bool _inWall, _slide, _jump_Wall;
    [SerializeField]
    float _strengthJump_WallX, _strengthJump_WallY, _timeJump_Wall, _strengthDamage_Y, _strengthDamage_X, cooldown_damage_max = 1.2f;



    public Skills skill;

    public Vector2 damage_rebote;
    public float time_enable;
    public float cooldown_damage;

    Iinteract interact;
    bool interactuo;
    public LayerMask interact_mask;

    public override void Spawned()
    {
        //Gamemanager.instance.AddToList(this);
        if (!HasStateAuthority) return;
        //if (Camera.main.TryGetComponent(out Camera_Follow follow)) follow.CameraCine(this);
        camara = Camera.main;

        Cambio_color(Runner.LocalPlayer);
    }

    private void Update()
    {
        if (!HasStateAuthority) return;
        if (controlEnabled)
        {
            move_horizontal = Input.GetAxisRaw("Horizontal") * _speed;
            cooldown_damage = cooldown_damage <= cooldown_damage_max ? cooldown_damage += Runner.DeltaTime : cooldown_damage;
            targetVelocity = Vector2.zero;
            ComputeVelocity();
            UpdateJumpState();
            Jump2D();
            Rotation_AIM(aim);
        }
        if (skill != null && Input.GetKeyDown(KeyCode.Mouse0)) Habilidad_Skill();
        Confirmar();
    }
    public override void FixedUpdateNetwork()
    {
        if (controlEnabled)
        {
            Movement2D(move_horizontal * Runner.DeltaTime);
            GravityPlayer();
            CheckJumpWall2D();
        }
    }

    public void Movement2D(float move)
    {
        if (controlEnabled)
        {
            velocity2.x = Input.GetAxisRaw("Horizontal");

            if (!_slide)
            {
                Vector3 speed_move = new Vector3(move, _net_rb2D.Rigidbody.velocity.y);
                _net_rb2D.Rigidbody.velocity = Vector3.SmoothDamp(_net_rb2D.Rigidbody.velocity, speed_move, ref velocity, _smooth_move);
            }
            if (move_horizontal > 0.01f) spriteRenderer.flipX = false;
            else if (move_horizontal < -0.01f) spriteRenderer.flipX = true;
        }
        else velocity2.x = 0f;
    }

    public void Jump2D()
    {
        if (jumpState == JumpState.Grounded && Input.GetKeyDown(KeyCode.W)) jumpState = JumpState.PrepareToJump;
        else if (Input.GetKeyUp(KeyCode.W)) stopJump = true;

        if (Input.GetKeyDown(KeyCode.W) && _inWall && _slide) JumpWall2D();
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

    public void UpdateJumpState()
    {
        Jump = false;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                Jump = true;
                stopJump = false;
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
        if (velocity2.y < 0) velocity2 += gravityModifier * Physics2D.gravity * Runner.DeltaTime;
        else velocity2 += Physics2D.gravity * Runner.DeltaTime;

        velocity2.x = targetVelocity.x;

        Grounded = false;

        var deltaPosition = velocity2 * Runner.DeltaTime;

        var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        var move = moveAlongGround * deltaPosition.x;

        PerformMovement(move, false);

        move = Vector2.up * deltaPosition.y;

        PerformMovement(move, true);
    }

    public void ComputeVelocity()
    {
        if (Jump && Grounded)
        {
            velocity2.y = jumpTakeOffSpeed * jumpModifier;
            Jump = false;
        }
        else if (stopJump)
        {
            stopJump = false;
            if (velocity2.y > 0) velocity2.y *= jumpDeceleration;
        }
        anim.SetBool("grounded", Grounded);
        anim.SetFloat("velocityX", Mathf.Abs(velocity.x) / _speed);
        targetVelocity = velocity2 * _speed;
    }

    public void CheckJumpWall2D()
    {
        _slide = !Grounded && _inWall && move_horizontal != 0 ? _slide = true : _slide = false;
        gravityModifier = _slide ? gravityModifier = .3f : gravityModifier = 1f;
        _inWall = Physics2D.OverlapBox(controll_wall.position, shell_wall, 0f, Wall);
        targetVelocity = velocity2 * _speed;
    }

    public void JumpWall2D()
    {
        _inWall = false;
        if (spriteRenderer.flipX)
            _net_rb2D.Rigidbody.velocity = new Vector2(_strengthJump_WallX * -move_horizontal, velocity2.y = _strengthJump_WallY);
        else _net_rb2D.Rigidbody.velocity = new Vector2(_strengthJump_WallX * move_horizontal, velocity2.y = _strengthJump_WallY);
        StartCoroutine(ChangeJumpWall2D());
    }

    IEnumerator ChangeJumpWall2D()
    {
        _jump_Wall = true;
        yield return new WaitForSeconds(_timeJump_Wall);
        _jump_Wall = false;
    }

    public void Rotation_AIM(GameObject aim)
    {
        puntero = camara.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(puntero.y - aim.transform.position.y, puntero.x - aim.transform.position.x);
        float angleGrad = (180 / Mathf.PI) * angleRad - 90;
        aim.transform.rotation = Quaternion.Euler(0, 0, angleGrad);
    }

    public void Rebote()
    {
        if (!spriteRenderer.flipX) _net_rb2D.Rigidbody.velocity = new Vector2(-damage_rebote.x * _strengthDamage_X, velocity2.y = damage_rebote.y);
        else _net_rb2D.Rigidbody.velocity = new Vector2(damage_rebote.x * _strengthDamage_X, velocity2.y = damage_rebote.y);
    }

    public IEnumerator Lose_controll()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        _speed = 0;
        yield return new WaitForSeconds(time_enable);
        _speed = 450;
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
                    //interact.interact(this);
                }
                break;
        }
    }

    public void Cambio_color(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            spriteRenderer.color = Color.cyan;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    }

    public void Habilidad_Skill()
    {
        Runner.Spawn(skill, verdadero_aim.transform.position, verdadero_aim.transform.rotation);
        //skill.Habilidad(this);
        skill = null;
        aim.SetActive(false);
    }

    //public void Revancha() => Gamemanager.instance.revancha.Add(this);

    //private void OnApplicationQuit()
    //{
    //    Gamemanager.instance.RemoveFromList(Runner.LocalPlayer);
    //    Gamemanager.instance.players2.Remove(this);
    //}




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controll_wall.position, shell_wall);
    }
}