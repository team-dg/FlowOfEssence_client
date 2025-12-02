using UnityEngine;
using System.Collections;

public class MinionCrate : MonoBehaviour
{
    public GameObject minionPrefab;  // 미니언 프리팹
    private float spawnInterval = 20f;  // 20초마다 생성
    private int minionsPerWave = 6;  // 한 번에 생성할 미니언 수
    private float timeBetweenMinions = 0.5f;  // 미니언 사이의 간격 (1초)

    void Start()
    {
        StartCoroutine(SpawnMinions());
    }

    IEnumerator SpawnMinions()
    {
        
        while (true)  // 무한 반복 (게임 내내 미니언 생성)
        {
            // 6마리의 미니언을 1초 간격으로 생성
            for (int i = 0; i < minionsPerWave; i++)
            {
                print("실행됨");
                Instantiate(minionPrefab, transform.position, transform.rotation);  // 미니언 생성
                yield return new WaitForSeconds(timeBetweenMinions);  // 1초 대기 후 다음 미니언 생성
            }
            yield return new WaitForSeconds(spawnInterval);//20초 대기
        }
    }
}
