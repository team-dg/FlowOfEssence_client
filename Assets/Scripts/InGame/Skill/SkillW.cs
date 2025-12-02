using Unity.VisualScripting;
using UnityEngine;

public class SkillW : SkillMove
{
    private bool sig = false;
    private GameObject collision;
    protected override void Update()
    {
        base.Update();

        if (moveDistance > 12)
        {
            Destroy(gameObject);
        }

        if (sig && collision != null && collision.CompareTag("Enemy")) 
        {
            print("지속");
            gameObject.transform.position = collision.transform.position;//4초 뒤에 파괴
            Destroy(gameObject, 4f);
        }
        
    }
    private void OnCollisionEnter(Collision temp)
    {
        if (!sig&& temp != null && temp.gameObject.CompareTag("Enemy"))
        {
            print("W실행");
            
            sig = true;
            collision = temp.gameObject;
            print(collision.tag);
            //Destroy(gameObject); 
        }
    }
}
