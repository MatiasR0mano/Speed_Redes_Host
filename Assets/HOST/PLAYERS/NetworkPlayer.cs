using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using UnityEngine;

public enum JumpState
{
    Grounded, PrepareToJump, Jumping, InFlight, Landed
}

[RequireComponent(typeof(LocalInputs))]
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }
    public LocalInputs Inputs { get; private set; }

    public Transform pos;
    public NetworkRigidbody2D _net_rb2D;
    public bool controlEnabled = true, ganador, i_am_host;
    public JumpState jumpState = JumpState.Grounded;
    Vector3 puntero;
    public Camera camara;
    public GameObject aim, verdadero_aim;
    [Header("Move2d")]
    public SpriteRenderer spriteRenderer;
    public float _speed;
    [Header("Jump2D")]
    public bool Jump = false;
    public bool Grounded = false, stopJump, double_jump, stop_double_jump;
    public float jumpTakeOffSpeed, jumpModifier, jump_time;

    [Header("WallJump2D")]
    public LayerMask Wall;
    [SerializeField] Transform controll_wall, controll_ground;
    [SerializeField] Vector3 shell_wall, shell_ground;
    public Vector2 strengthJump;
    public bool _inWall, _slide, _jump_Wall, takedamage;
    public float cooldown_damage_max, jump_time_wall, _timeJump_Wall;

    public Skills skill;

    public Vector2 damage_rebote;
    public float time_enable;
    public float cooldown_damage;

    Iinteract interact;
    public bool interactuo;
    public bool permito_irme;
    public bool permito_irme2;
    public LayerMask interact_mask, ground_mask;

    public event Action<float> OnMovement = delegate { };

    //SFX
    public AudioSource audSource;
    public AudioClip[] audClips;

    public override void Spawned()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        Gamemanager.instance.RPC_AddToList(this);
        if (HasInputAuthority)
        {
            if (Camera.main.TryGetComponent(out Camera_Follow follow)) follow.CameraCine(this);
            Cambio_color(Runner.LocalPlayer);
        }
        if (HasStateAuthority) i_am_host = true;
        camara = Camera.main;
        Inputs = GetComponent<LocalInputs>();

        if (Object.HasInputAuthority)
        {
            Local = this;
            Inputs.enabled = true;
        }
        else Inputs.enabled = false;
    }

    private void Update()
    {
        if (controlEnabled)
        {
            cooldown_damage = cooldown_damage < cooldown_damage_max ? cooldown_damage += Runner.DeltaTime : cooldown_damage;
            Confirmar();
        }
    }

    public void Move2D(float xAxi)
    {
        if (controlEnabled)
        {
            if (xAxi > 0.01f)
            {
                spriteRenderer.flipX = false;
                controll_wall.position = new Vector2(transform.position.x + 1f, transform.position.y);
            }
            else if (xAxi < -0.01f)
            {
                spriteRenderer.flipX = true;
                controll_wall.position = new Vector2(transform.position.x - 1f, transform.position.y);
            }

            if (xAxi != 0)
            {
                transform.right = Vector2.right * Mathf.Sign(xAxi);
                _net_rb2D.Rigidbody.velocity += Vector2.right * (xAxi * _speed * 10 * Runner.DeltaTime);

                if (Mathf.Abs(_net_rb2D.Rigidbody.velocity.x) > _speed)
                {
                    var velocity = Vector3.ClampMagnitude(_net_rb2D.Rigidbody.velocity, _speed);
                    velocity.y = _net_rb2D.Rigidbody.velocity.y;
                    _net_rb2D.Rigidbody.velocity = velocity;
                }
                OnMovement(xAxi);
            }
            else
            {
                var velocity = _net_rb2D.Rigidbody.velocity;
                velocity.x = 0;
                _net_rb2D.Rigidbody.velocity = velocity;
                OnMovement(0);
            }
        }
    }

    public void Jump2D(bool input)
    {
        if (controlEnabled)
        {
            Jump = input;
            if (!takedamage && !_inWall)
            {
                if (jumpState == JumpState.Grounded && Jump) jumpState = JumpState.PrepareToJump;
                else if (input) stopJump = true;
            }
            if (_inWall) _net_rb2D.Rigidbody.gravityScale = 2f;
            else _net_rb2D.Rigidbody.gravityScale = 5f;
        }
    }

    public void UpdateJumpState(bool input)
    {
        if (controlEnabled)
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
    }

    public void Jump_model(bool input, bool input_double)
    {
        if (controlEnabled)
        {
            _inWall = Physics2D.OverlapBox(controll_wall.position, shell_wall, 0f, Wall);
            Jump = input;
            double_jump = input_double;
            jump_time = jump_time < 1f ? jump_time += Runner.DeltaTime : jump_time;
            if (Jump && Grounded && jump_time >= 1f)
            {
                if (!_inWall) _net_rb2D.Rigidbody.AddForce(Vector2.up * jumpTakeOffSpeed, ForceMode2D.Impulse);
                stop_double_jump = false;
                Jump = false;
                jump_time = 0f;
            }
            if (double_jump && !Grounded && !stop_double_jump && jump_time >= .25f)
            {
                if (!_inWall) _net_rb2D.Rigidbody.AddForce(Vector2.up * (jumpTakeOffSpeed * 1.2f) * (jumpModifier * 1.2f), ForceMode2D.Impulse);
                double_jump = false;
                stop_double_jump = true;
                jump_time = 0f;
            }
            else if (stopJump) stopJump = false;
        }
    }

    public void JumpWall2D(bool input)
    {
        _jump_Wall = input;
        _timeJump_Wall = _timeJump_Wall < .8f ? _timeJump_Wall += Runner.DeltaTime : _timeJump_Wall;
        if (_jump_Wall && _inWall && _timeJump_Wall >= .8f)
        {
            if (spriteRenderer.flipX)
            {
                StartCoroutine(Jump_Wall());
                _net_rb2D.Rigidbody.velocity = new Vector2(strengthJump.x, strengthJump.y);
                _timeJump_Wall = 0f;
            }
            else
            {
                StartCoroutine(Jump_Wall());
                _net_rb2D.Rigidbody.velocity = new Vector2(-strengthJump.x, strengthJump.y);
                _timeJump_Wall = 0f;
            }
        }
    }

    public IEnumerator Jump_Wall()
    {
        spriteRenderer.flipX = spriteRenderer.flipX ? spriteRenderer.flipX = false : spriteRenderer.flipX = true;
        _speed = 0f;
        takedamage = true;
        yield return new WaitForSeconds(.5f);
        _speed = 20f;
        takedamage = false;
    }

    public void Rotation_AIM(GameObject aim, Vector3 mouse)
    {
        puntero = camara.ScreenToWorldPoint(mouse);
        float angleRad = Mathf.Atan2(puntero.y - aim.transform.position.y, puntero.x - aim.transform.position.x);
        float angleGrad = (180 / Mathf.PI) * angleRad - 90;
        aim.transform.rotation = Quaternion.Euler(0, 0, angleGrad);
    }

    public IEnumerator Lose_controll()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        _speed = 0;
        takedamage = true;
        yield return new WaitForSeconds(time_enable);
        _speed = 20;
        takedamage = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    public void TakeDamage()
    {
        if (cooldown_damage >= cooldown_damage_max)
        {
            StartCoroutine(Lose_controll());
            if (spriteRenderer.flipX) _net_rb2D.Rigidbody.velocity = new Vector2(damage_rebote.x, damage_rebote.y);
            else _net_rb2D.Rigidbody.velocity = new Vector2(-damage_rebote.x, damage_rebote.y);
            cooldown_damage = 0f;
            RPC_PlayXsoundClip(3);
        }
    }

    public void Confirmar()
    {
        interactuo = Physics2D.OverlapBox(controll_wall.position, shell_wall, 0f, interact_mask);
        Grounded = Physics2D.OverlapBox(controll_ground.position, shell_ground, 0f, ground_mask);
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

    public void Habilidad_Skill(Transform aim2)
    {
        aim.SetActive(false);
        skill.Habilidad(this);
        Runner.Spawn(skill, aim2.position, aim2.rotation);
        RPC_PlayXsoundClip(1);
        //skill = null;
    }

    public void TeleportPlayer() => _net_rb2D.Teleport(pos.position);

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DisconnectPlayer(bool permito)
    {
        //permito_irme = permito;
        permito = true;
        if (!i_am_host && permito)
        {
            Debug.Log("despawn");
            Runner.Despawn(Object);
        }
        if (!Object.HasInputAuthority && permito)
        {
            Runner.Despawn(Object);
            Runner.Disconnect(Object.InputAuthority);
            Debug.Log("hasinput");
        }
        else
        {
            Debug.Log("Shutdown");
            Runner.Shutdown();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_F11(bool permito)
    {
        permito_irme = permito;
        if (!Object.HasInputAuthority && permito_irme)
        {
            Runner.Disconnect(Object.InputAuthority);
            Runner.Despawn(Object);
            Debug.Log(permito_irme);
        }
    }

    //SFX try

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayXsoundClip (int soundClip)
    {
        audSource.PlayOneShot(audClips[soundClip]);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controll_wall.position, shell_wall);
        Gizmos.DrawWireCube(controll_ground.position, shell_ground);
    }

}
