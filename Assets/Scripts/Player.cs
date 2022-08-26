using System.Collections;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] float _movementSpeed = 5.0f;

    [Header("Combat Settings")]
    [SerializeField] float _horizontalSensitivity = 1.0f;
    [SerializeField] float _fireRate = 0.3f;
    [SerializeField] LayerMask _enemyLayerMask;
    [SerializeField] ParticleSystem _muzzleFlash;

    [SerializeField] int _maxHealth = 100;
    [SerializeField] Behaviour[] _hideOnDeath;

    NetworkAnimator _networkAnimator;
    Animator _animator;
    Rigidbody _rigidbody;

    int _currentHealth;
    float _yRotation;
    float _timer;

    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public Behaviour[] HideOnDeath { get => _hideOnDeath; set => _hideOnDeath = value; }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _networkAnimator = GetComponent<NetworkAnimator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currentHealth = _maxHealth;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MovePlayer(horizontal, vertical);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (!isLocalPlayer)
            return;

        float mouseX = Input.GetAxis("Mouse X") * _horizontalSensitivity;
        RotatePlayer(mouseX);


        _timer += Time.deltaTime;

        if (Input.GetMouseButton(0) && _fireRate <= _timer)
            Fire();
    }

    void MovePlayer(float horizontal, float vertical)
    {
        Vector3 direction = (transform.forward * vertical + transform.right * horizontal);
        if (horizontal != 0 && vertical != 0)
            direction.Normalize();

        _rigidbody.MovePosition(transform.position + (direction * _movementSpeed * Time.deltaTime));

        AnimOnMovement(direction);
    }

    void RotatePlayer(float mouseX)
    {
        _yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(0, _yRotation, 0);
    }

    void Fire()
    {
        _timer = 0;

        StartCoroutine(AnimOnFire());

        RaycastHit rayHit;
        bool characterHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHit, 1000f, _enemyLayerMask);

        if (characterHit)
        {
            Player target = rayHit.transform.GetComponent<IDamageable>() as Player;
            CmdInflictDamage(target, 10);
        }
    }

    void AnimOnMovement(Vector3 direction)
    {
        _animator.SetBool("IsMoving", direction != Vector3.zero);
    }

    IEnumerator AnimOnFire()
    {
        _networkAnimator.SetTrigger("IsFired");
        yield return new WaitForSeconds(0.17f);
        _muzzleFlash.Play();
    }

    [Command]
    public void CmdInflictDamage(Player target, int damage)
    {
        int targetHealth = target.CurrentHealth -= damage;
        print(target.CurrentHealth);

        if (target.CurrentHealth <= 0)
        {
            target.Die(target);
            target.CurrentHealth = _maxHealth;
        }
    }

    [ClientRpc]
    public void Die(Player target)
    {
        if (!isLocalPlayer)
            return;

        foreach (var behaviour in HideOnDeath)
        {
            behaviour.enabled = false;
        }

        target.StartCoroutine(DeathCor(target));
    }

    IEnumerator DeathCor(Player target)
    {
        if (isLocalPlayer)
        {
            _networkAnimator.SetTrigger("IsDied");
            yield return new WaitForSeconds(2.5f);
            Respawn(target);
        }
    }

    public void Respawn(Player target)
    {
        if (!isLocalPlayer)
            return;

        foreach (var behaviour in HideOnDeath)
        {
            behaviour.enabled = true;
        }

        NetworkManagerLobby room = NetworkManager.singleton as NetworkManagerLobby;
        transform.position = room.GetStartPosition().position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
