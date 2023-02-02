using DG.Tweening;
using System.Collections;
using UnityEngine;
public abstract class BallBase :MonoBehaviour
{
    [SerializeField] long perSecondIncome;
    [SerializeField] float damage;

    private float initDamage;

    public BallType ballType;
    public BallType nextBallType;

    public Animator anim;

    public GameObject bouncyObj;

    public GameObject ballObj;

    [SerializeField] ParticleSystem hitEffect;

    [SerializeField] Color color;

    [SerializeField] TrailRenderer trail;

    [SerializeField] float powerIncreaseFactory;
    int powerLevel = 1 ;

    private void OnEnable()
    {
        EventManager.OnSettingHeight += DelayAnim;

        EventManager.OnStartingGame += StartBallAnim;
    }

    private void OnDisable()
    {
        EventManager.OnSettingHeight -= DelayAnim;

        EventManager.OnStartingGame -= StartBallAnim;
    }

    private void Awake()
    {
        initDamage = damage;
    }

    private void Update()
    {
        SetFlameEffect();
    }

    public void Attack(WallBase wall)
    {
        wall.TakeDamage(damage);
        hitEffect.Play();
    }

    public void Replace(Vector3 pos)
    { 
        transform.DOLocalMove(pos, 0.5f);
    }

    public void Reset()
    {
        anim.enabled = true; 

        bouncyObj.transform.localPosition = Vector3.zero;

        damage = initDamage;
    }

    public void ActivatePassiveIncome()
    {
        EventManager.IncrementPerSecondMoney?.Invoke(perSecondIncome);
    }

    public void DeactivatePassiveIncome()
    {
        EventManager.DecrementPerSecondMoney?.Invoke(perSecondIncome);
    }

    private void SetFlameEffect()
    {
        trail.startWidth = (Time.timeScale / 5);
    }
    
    private void DelayAnim()
    {
        StartCoroutine(IEAnimationHandler());
    }

    private IEnumerator IEAnimationHandler()
    {
        yield return new WaitForSeconds(1);
        anim.SetFloat("Speed", 1);
    }

    private void StartBallAnim()
    {
        anim.enabled = true;
    }
}
