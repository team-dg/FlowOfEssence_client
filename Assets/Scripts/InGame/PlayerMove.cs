using UnityEngine;
using UnityEngine.AI;
public class PlayerMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 destination;

    private bool isMove;
    private bool isSkill = false;

    public Camera cam;
    public GameObject Mouseclick;
    public GameObject HPbar;
    public int 일반공격범위;

    Abilities abilities;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        abilities= GetComponent<Abilities>();
        agent.updateRotation = false;//Nav로 로테이션이 변경되는 것을 방지
        agent.acceleration = float.MaxValue;//가속도를 최대로 해서 더 부드럽게
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !abilities.GetskillCooltime(5))
        {
            isMove = false;
            agent.ResetPath();//이동을 취소
            animator.SetBool("IsMove", false);
            abilities.스킬사용(5);
        }
        if (isSkill==false)
        {
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Alpha1))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소 
                animator.SetInteger("IsCtrl", 1);
                animator.SetBool("IsMove", false);
            }
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Alpha2))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetInteger("IsCtrl", 2);
                animator.SetBool("IsMove", false);
            }
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Alpha3))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetInteger("IsCtrl", 3);
                animator.SetBool("IsMove", false);
            }
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Alpha4))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetInteger("IsCtrl", 4);
                animator.SetBool("IsMove", false);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                if (!currentState.IsTag("Return"))
                {
                    animator.SetTrigger("Recall");
                    animator.SetBool("IsMove", false);
                }
            }
            if (Input.GetKeyDown(KeyCode.Q)&& !abilities.GetskillCooltime(0))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetTrigger("IsSkill1");
                animator.SetBool("IsMove", false);
                isSkill = true;
                abilities.스킬사용(0);
            }
            if (Input.GetKeyDown(KeyCode.W) && !abilities.GetskillCooltime(1))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetTrigger("IsSkill2");
                animator.SetBool("IsMove", false);
                isSkill = true;
                abilities.스킬사용(1);
            }

            if (Input.GetKeyDown(KeyCode.E) && !abilities.GetskillCooltime(2))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetTrigger("IsSkill3");
                animator.SetBool("IsMove", false);
                isSkill = true;
                abilities.스킬사용(2);
            }

            if (Input.GetKeyDown(KeyCode.R) && !abilities.GetskillCooltime(3))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetTrigger("IsSkill4");
                animator.SetBool("IsMove", false);
                isSkill = true;
                abilities.스킬사용(3);
            }
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                isMove = false;
                agent.ResetPath();//이동을 취소
                animator.SetInteger("IsAttack", 1);
            }
            if (Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.X))
            {
                animator.SetInteger("IsAttack", 0);
                animator.SetInteger("IsCtrl", 0);
                agent.velocity = Vector3.zero;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                // 첫 번째 Raycast: 적을 클릭했는지 확인
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, hit.collider.transform.position);

                    if (hit.collider.CompareTag("Enemy")&& distanceToEnemy <= 일반공격범위)//여기 if문에서 플레이어 중심으로부터 원방향으로 일정 거리에 있는 상대만 공격하고싶어
                    {
                        isMove = false;
                        agent.ResetPath();//이동을 취소
                        animator.SetTrigger("CanAttack");
                        Debug.Log("때린다");

                        Vector3 targetPosition = hit.point;
                        Vector3 direction = targetPosition - transform.position;

                        // 회전할 때 Y축을 고정 (캐릭터가 위아래로 회전하지 않도록)//이거 왜 안해도 되는걸까
                        //direction.y = 0;

                        // 캐릭터가 마우스 방향을 바라보도록 회전
                        transform.forward = direction;
                    }
                    else
                    {
                        Instantiate(Mouseclick).transform.position = hit.point;
                        // 적이 아니더라도 클릭 위치에 오브젝트가 있을 경우 이동
                        SetDestination(hit.point);
                    }
                }
                else
                {
                    // 두 번째 Raycast: 아무 오브젝트에도 충돌하지 않았을 경우 클릭 위치로 이동
                    if (Physics.Raycast(ray, out hit))
                    {
                        Instantiate(Mouseclick).transform.position = hit.point; // 클릭 모양 생성
                        SetDestination(hit.point); // 목적지 설정
                    }
                }
            }
        }

        Move();
        Vector3 hpPosition = new Vector3(0,2.5f,0)+transform.position;//Hp바가 밑으로 내려가서 2.5만큼 올려줌
        HPbar.transform.position = hpPosition;
    }
    private void SetDestination(Vector3 dest)
    {
        if (agent != null && agent.isOnNavMesh) // NavMesh 위에 있는지 확인
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(dest, path) && path.status == NavMeshPathStatus.PathComplete) // 경로가 유효한지 확인
            {
                agent.SetPath(path);
                destination = dest;
                if(isMove==false)//만약 움직이는 상태가 아니라면
                {
                    animator.SetTrigger("CanMove");//anyState에서 트리거를 발동해서 바로 Move로 이동
                }
                isMove = true;
                animator.SetBool("IsMove", true);
            }
            else
            {
                Debug.LogWarning("Unable to calculate path or incomplete path.");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent is not on the NavMesh.");
        }

    }
    private void Move()
    {
        if (isMove)
        {
            if (Vector3.Distance(transform.position, destination) <= 0.1f)
            {
                isMove = false;
                animator.SetBool("IsMove", false);
                return;
            }
            var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
            transform.forward = dir;//캐릭터의 rotation을 이동하는 방향으로 설정
        }
    }

    public void AnimationCtrlEnd()
    {
        Debug.Log("AnimationCtrlEnd 함수가 호출되었습니다");
        animator.SetInteger("IsCtrl", 0);
    }
    public void AnimationSkillEnd()
    {
        Debug.Log("AnimationSkillEnd 함수가 호출되었습니다");
        isSkill = false;
    }
    public void AnimationAttackEnd()
    {
        animator.SetInteger("IsAttack", 0);
    }

    public bool GetisSkill()
    {
        return isSkill;
    }
    public void 귀환()
    {
        gameObject.transform.position = new Vector3(-4.1f, 2.46f, 107.78f);
    }
}
