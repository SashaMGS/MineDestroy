using System.Collections;
using UnityEngine;


public class PlController : MonoBehaviour
{
    public bool isMobile = false;
    [Header("Возможности персонажа")]
    [Tooltip("Опциональная возможность ходьбы персонажа")]
    public bool canWalkPlayer = true;
    [Tooltip("Опциональная возможность бега персонажа")]
    public bool canRunPlayer = true;
    [Tooltip("Опциональная возможность приседания персонажа")]
    public bool canCrouchPlayer = true;
    [Tooltip("Опциональная возможность лежать персонажа")]
    public bool canLiePlayer = true;
    [Tooltip("Можно ли использовать дабл прыжок?")]
    public bool canDoubleJump = false;
    [Tooltip("Можно ли летать?")]
    public bool canFly = false;
    [Tooltip("Гравитация прыжка")]
    public float gravityFly = -15f;

    [Header("Переменные скорости персонажа")]
    [Tooltip("Скорость ходьбы")]
    public float walkSpeed = 5f; // Скорость ходьбы
    [Tooltip("Скорость бега")]
    public float runSpeed = 10f; // Скорость бега
    [Tooltip(" Скорость в присяди")]
    public float crouchSpeed = 2.5f;
    [Tooltip(" Скорость в присяди")]
    public float lieSpeed = 1f;

    [Header("Переменные гравитации персонажа")]
    [Tooltip("Сила прыжка.")]
    [SerializeField] private float jumpForce = 6f;
    [Tooltip("Гравитацияю. Сила притяжения игрока к земле")]
    [SerializeField] public float gravity = -12f; // Скорость падения игрока (Гравитация)

    [Header("Переменные стамины персонажа")]
    [Tooltip("Продолжительность бега")]
    public float staminaCurrent = 1f; // Продолжительность бега
    [Tooltip("Минимальное значение, при котором возможно начать бег")]
    public float speedCanRunStamina = 0.15f; // Минимальное значение, при котором возможно начать бег
    [Tooltip("Максимальная продолжительность бега")]
    public float staminaFull = 1f; // Максимальная продолжительность бега
    [Tooltip("Скорость усталости при беге")]
    public float speedStaminaMinus = 0.1f; // Скорость усталости при беге
    [Tooltip("Скорость восполнения выносливости")]
    public float speedStaminaPlus = 0.1f; // Скорость восполнения выносливости

    [Header("Кнопки передвижения персонажа")]
    [Tooltip("Кнопка прыжка")]
    public KeyCode jumpKeyCode = KeyCode.Space;
    [Tooltip("Кнопка приседания")]
    public KeyCode crouchKeyCode = KeyCode.C;
    [Tooltip("Кнопка лечь")]
    public KeyCode lieKeyCode = KeyCode.X;

    public Animator playerAnim;
    public GameObject rotationObj;
    public float rotationSpeed = 5f; // Скорость поворота

    [HideInInspector]
    public bool canRun = true; // Игрок может бежать?
    [HideInInspector]
    public bool isRun = false; // Игрок бежит?
    private bool isWalk = false;
    private bool isCrouching = false;
    private bool isUpBlock = false;
    private bool isLie = false;
    public bool doubleJumpCur = false;

    private AudioSource audioSource;

    public float distanceRayDown = 1.5f;
    public float distanceRayAround = 0.5f;
    public LayerMask layerMaskGround;
    private float distanceRayUp = 1.5f;

    private CharacterController controller; // Компонент Unity, через которые осуществляется движение игрока
    public HeadBob headBobScript; // Скрипт позволяющий оживить движение головы игрока
    //[HideInInspector]
    public bool isOnGround = false;
    private float playerSpeed = 5f; // Текущая скорость игрока
    private Vector3 velocity; // Вектор движения игрока по оси Y
    private float moveHorizontal; // x
    private float moveVertical; // z
    private Vector3 direction = Vector3.zero;
    //public VariableJoystick variableJoystick; // Скрипт джойстика для управления с телефона (Необходим для управления и для пк) (New)

    void Start()
    {
        if (canFly)
            gravity = gravityFly;
        //
        if (GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(SoundWalk());
        }
        controller = GetComponent<CharacterController>();
        if (GetComponentInChildren<HeadBob>())
            headBobScript = GetComponentInChildren<HeadBob>();
    }

    void Update()
    {
        if (Input.GetKey(jumpKeyCode) && isOnGround && !isCrouching && !isLie && staminaCurrent >= speedStaminaMinus)
        {
            if (!isRun)
                staminaCurrent -= speedStaminaMinus;
            else
                staminaCurrent -= speedStaminaMinus / 2;
            doubleJumpCur = true;
            velocity.y = jumpForce;
        }
        if (Input.GetKey(jumpKeyCode) && !isOnGround && !isCrouching && !isLie && doubleJumpCur && canDoubleJump && staminaCurrent >= speedStaminaMinus)
        {
            if (!isRun)
                staminaCurrent -= speedStaminaMinus;
            else
                staminaCurrent -= speedStaminaMinus / 2;
            doubleJumpCur = false;
            velocity.y = jumpForce;
        }
        if (Input.GetKey(crouchKeyCode) && isOnGround)
            Crouch();
        if (Input.GetKey(lieKeyCode) && isOnGround)
            Lie();

        Walk();
        Run();
        RotatePlayer();
        StaminaVoid();
        Events();

        if (playerAnim)
        {
            playerAnim.SetBool("Run", isRun);
            playerAnim.SetBool("Walk", isWalk);
            playerAnim.SetBool("isOnGround", isOnGround);
        }
    }
    private void FixedUpdate()
    {
        GravityVoid();
    }

    void Events()
    {
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceRayUp))
        {
            isUpBlock = true;
            Debug.DrawRay(transform.position, transform.up, Color.red, distanceRayUp);
        }
        else
        {
            isUpBlock = false;
            Debug.DrawRay(transform.position, transform.up, Color.green, distanceRayUp);
        }

        if (Physics.Raycast(transform.position, -transform.up, out hit, distanceRayDown, layerMaskGround))
        {
            isOnGround = true;
            Debug.DrawRay(transform.position, -transform.up, Color.green, distanceRayDown);
        }
        else
        {
            isOnGround = false;
            Debug.DrawRay(transform.position, -transform.up, Color.red, distanceRayDown);
        }
    }

    void Lie()
    {
        if (isRun && canLiePlayer)
        {
            isRun = false;
            headBobScript.isRun = false;
        }

        if (isLie && !isUpBlock)
        {
            playerSpeed = walkSpeed;
            isLie = false;
            controller.height = 2;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (!isLie && canLiePlayer)
        {
            playerSpeed = lieSpeed;
            isLie = true;
            controller.height = 0.5f;
            transform.localScale = new Vector3(1f, 0.25f, 1f);
        }
    }

    void Crouch()
    {
        if (isRun && canCrouchPlayer)
        {
            isRun = false;
            headBobScript.isRun = false;
        }

        if (isCrouching && !isUpBlock)
        {
            playerSpeed = walkSpeed;
            isCrouching = false;
            controller.height = 2;
        }
        else if (!isCrouching && canCrouchPlayer)
        {
            playerSpeed = crouchSpeed;
            isCrouching = true;
            controller.height = 1;
        }
    }

    void Walk()
    {
        if (canWalkPlayer)
        {
            if (!isMobile)
            {
                moveHorizontal = Input.GetAxis("Horizontal"); // x
                moveVertical = Input.GetAxis("Vertical"); // z
            }
            else
            {

            }
        }
        if ((moveHorizontal != 0 || moveVertical != 0) && canWalkPlayer)
            isWalk = true;
        if (moveHorizontal == 0 && moveVertical == 0)
            isWalk = false;

        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        controller.Move(move * playerSpeed * Time.deltaTime); // Двигаем игрока относительно заданного вектора
    }

    void GravityVoid()
    {
        if (!isOnGround)
            velocity.y += gravity * Time.deltaTime; // Применяем гравитацию к игроку

        controller.Move(velocity * Time.deltaTime); // Двигаем игрока относительно оси Y
    }

    void StaminaVoid()
    {
        if (staminaCurrent < 0)
        {
            staminaCurrent = 0;
            canRun = false;
            isRun = false;
            headBobScript.isRun = false;
            playerSpeed = walkSpeed;
        }

        if (isRun && staminaCurrent > 0)
            staminaCurrent -= speedStaminaMinus * Time.deltaTime;

        if (!isRun && staminaCurrent < staminaFull)
            staminaCurrent += speedStaminaPlus * Time.deltaTime;

        if (staminaCurrent > staminaFull)
            staminaCurrent = staminaFull;

        if (staminaCurrent >= speedCanRunStamina)
            canRun = true;
    }
    void Run()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canRun && staminaCurrent > speedCanRunStamina
            && isCrouching && !isUpBlock && !isLie && canRunPlayer && (moveHorizontal != 0 || moveVertical != 0))
        {
            isCrouching = false;
            controller.height = 2;
            playerSpeed = runSpeed;
            isRun = true;
            headBobScript.isRun = true;
        }
        else if (canRun && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !isCrouching && staminaCurrent > speedCanRunStamina && canRunPlayer && !isLie)
        {
            playerSpeed = runSpeed;
            isRun = true;
            headBobScript.isRun = true;
        }
        if ((Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) && !isCrouching && !isLie)
        {
            playerSpeed = walkSpeed;
            isRun = false;
            headBobScript.isRun = false;
        }
        else if (isRun && !isCrouching && !isLie && moveHorizontal == 0f && moveVertical == 0f)
        {
            playerSpeed = walkSpeed;
            isRun = false;
            headBobScript.isRun = false;
        }
    }

    private void RotatePlayer()
    {
        direction = Vector3.zero;

        if (Input.GetAxis("Vertical") > 0)
            direction += Vector3.forward;
        if (Input.GetAxis("Vertical") < 0)
            direction += Vector3.back;

        if (Input.GetAxis("Horizontal") > 0)
            direction += Vector3.right;
        if (Input.GetAxis("Horizontal") < 0)
            direction += Vector3.left;
        /*
        if (Input.GetKey(KeyCode.W))
            direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            direction += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;
        */
        if (Vector3.Distance(transform.position, GetComponent<ShootingSystem>().GetNearPos()) > 10f)
        {
            GetComponent<ShootingSystem>()._isZooming = false;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rotationObj.transform.rotation = Quaternion.Slerp(rotationObj.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, GetComponent<ShootingSystem>().GetNearPos()) <= 10f)
        {
            GetComponent<ShootingSystem>()._isZooming = true;
            rotationObj.transform.LookAt(GetComponent<ShootingSystem>().GetNearPos());
            rotationObj.transform.rotation = new Quaternion(0f, rotationObj.transform.rotation.y, rotationObj.transform.rotation.z, rotationObj.transform.rotation.w);
        }
    }

    IEnumerator SoundWalk()
    {
        while (true)
        {
            if (isWalk && !isRun)
            {
                audioSource.Play();
                yield return new WaitForSeconds(0.5f);
            }
            if (isRun)
            {
                audioSource.Play();
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}