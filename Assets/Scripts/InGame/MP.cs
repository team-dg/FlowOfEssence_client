using UnityEngine;
using UnityEngine.UI;

public class MP : MonoBehaviour
{
    public int mp;
    public Slider playerSlider3DMP;
    Slider playerSlider2DMP;
    
    void Start()
    {
        playerSlider2DMP = GetComponent<Slider>();
        playerSlider2DMP.maxValue = mp;
        playerSlider3DMP.maxValue = mp;
    }
    void Update()
    {
        playerSlider2DMP.value = mp;
        playerSlider3DMP.value = playerSlider2DMP.value;
    }
}
