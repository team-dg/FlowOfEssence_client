using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Abilities : MonoBehaviour
{
    public Camera cam;
    public GameObject Skillposition;

    [Header("스킬1")]
    public float skillCooltime1 = 5f;
    public GameObject Q;
    
    [Header("스킬2")]
    public float skillCooltime2 = 5f;
    public GameObject W;

    [Header("스킬3")]
    public float skillCooltime3 = 5f;
    public GameObject E;

    [Header("스킬4")]
    public float skillCooltime4 = 5f;
    public GameObject R;

    [Header("스킬F")]
    public float skillCooltimeF = 5f;

    public Image[] skillImage; 
    private bool[] isCooltime = new bool[6];

    PlayerMove PlayerMove;
    

    private void Start()
    {
        for(int i=0; i<isCooltime.Length; i++)
        {
            isCooltime[i] = false;
            skillImage[i].fillAmount = 0;
        }
        PlayerMove = GetComponent<PlayerMove>();
    }
    void Update()
    {
        Skill1();
        Skill2();
        Skill3();
        Skill4();
        SkillF();
    }
    void Skill1()
    {
        if (isCooltime[0])
        {
            skillImage[0].fillAmount -= 1 / skillCooltime1 * Time.deltaTime;
            if (skillImage[0].fillAmount <= 0)
            {
                skillImage[0].fillAmount = 0;
                isCooltime[0] = false;
            }
        }
    }
    void Skill2()
    {
        if (isCooltime[1])
        {
            skillImage[1].fillAmount -= 1 / skillCooltime2 * Time.deltaTime;
            if (skillImage[1].fillAmount <= 0)
            {
                skillImage[1].fillAmount = 0;
                isCooltime[1] = false;
            }
        }
    }
    void Skill3()
    {
        if (isCooltime[2])
        {
            skillImage[2].fillAmount -= 1 / skillCooltime3 * Time.deltaTime;
            if (skillImage[2].fillAmount <= 0)
            {
                skillImage[2].fillAmount = 0;
                isCooltime[2] = false;
            }
        }
    }

    void Skill4()
    {
        if (isCooltime[3])
        {
            skillImage[3].fillAmount -= 1 / skillCooltime4 * Time.deltaTime;
            if (skillImage[3].fillAmount <= 0)
            {
                skillImage[3].fillAmount = 0;
                isCooltime[3] = false;
            }
        }
    }
    void SkillF()
    {
        if (isCooltime[5])
        {
            skillImage[5].fillAmount -= 1 / skillCooltime4 * Time.deltaTime;
            if (skillImage[5].fillAmount <= 0)
            {
                skillImage[5].fillAmount = 0;
                isCooltime[5] = false;
            }
        }
    }

    Quaternion skillRotation;//스킬을 눌렀을 때 좌표를 저장하고 애니메이션 중간에 생성
    Vector3 tempSkillPosition;
    void InstantiateSkill좌표(int i)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;

            // 회전할 때 Y축을 고정 (캐릭터가 위아래로 회전하지 않도록)//이거 왜 안해도 되는걸까
            direction.y = 0;

            // 캐릭터가 마우스 방향을 바라보도록 회전
            transform.forward = direction;
            if(i!=5)
            {
                tempSkillPosition = Skillposition.transform.position;
                skillRotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, 180);
            }
            
        }
    }

    void InstantiateSkill(GameObject Y)
    {
        Instantiate(Y, tempSkillPosition, skillRotation);
    }

    public void 스킬사용(int i)
    {
        if (!isCooltime[i])
        {
            isCooltime[i] = true;
            skillImage[i].fillAmount = 1;
            InstantiateSkill좌표(i);
        }

        if(i==5)
        {
            StartCoroutine(SmoothMove(transform.position, transform.position + transform.forward * 3.5f, 0.001f));
        }
    }

    public void 비전이동() // E스킬 사용 시 조금 앞으로 부드럽게 이동
    {
        float forwardDistance = 3.5f;  // 이동할 거리
        float duration = 0.01f;  // 이동할 시간 (0.05초 동안 이동)
        StartCoroutine(SmoothMove(transform.position, transform.position + transform.forward * forwardDistance, duration));
    } 

    // 코루틴: 시간이 지남에 따라 부드럽게 이동
    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 targetPosition, float duration)
    { 
        float elapsedTime = 0f;

        // duration 동안 점진적으로 이동
        while (elapsedTime < duration)
        {
            // 현재 시간을 기준으로 보간
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            // 경과 시간 증가
            elapsedTime += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 마지막 위치 설정 (정확히 목표 위치에 도달하도록)
        transform.position = targetPosition;
    }

    public bool GetskillCooltime(int i)
    {
        return isCooltime[i];
    }
}