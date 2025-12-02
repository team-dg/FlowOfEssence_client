using UnityEngine;
using UnityEngine.UI;
public class MInionHP : MonoBehaviour
{
    [SerializeField] private float maxHP;
    public float health;
    private Slider hpslider3D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpslider3D = GetComponentInChildren<Slider>();
        health = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        hpslider3D.value = health/maxHP;
        if(health<=0)
        {
            Destroy(gameObject);
        }
    }

    public void GetDamage(int damage)
    {
        health -= damage;
    }
}
