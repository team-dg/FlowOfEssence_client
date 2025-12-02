using UnityEngine;

public class SkillR : SkillMove
{
    protected override void Update()
    {
        base.Update();
        if (moveDistance > 500)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Enemy")
        {
            MInionHP minionHPscript = other.gameObject.GetComponentInChildren<MInionHP>();
            minionHPscript.GetDamage(10);
        }
    }
}
