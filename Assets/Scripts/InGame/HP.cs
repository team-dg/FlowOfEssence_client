using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    private float maxHP;
    private Slider playerSlider2D;
    public Slider playerSlider3D;
    public int health;
    public GameObject hpLine;    
    
    void Start()
    {
        playerSlider2D = GetComponent<Slider>();
        playerSlider2D.maxValue = health;
        playerSlider3D.maxValue = health;
        maxHP = health;//처음 체력이 max체력임
        GetHP();
    }

    // Update is called once per frame
    void Update()
    {
        playerSlider2D.value = health;
        playerSlider3D.value = playerSlider2D.value;
    }
    
    public void GetHP()
    {
        health += 100;
        float scaleX = (maxHP / 100) / (health / 100);
        hpLine.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
        foreach(Transform child in hpLine.transform)//자식 오브젝트의 X스케일을 조절해서 체력바를 구성함.
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
        }
        hpLine.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
    }
}
 