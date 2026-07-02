using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialMagicAttack : MonoBehaviour
{
    public List<GameObject> unitsAroundTarget = new();
    public GameObject aoeEffect;

    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (gameObject.GetComponent<Attackcontroller>().targettoAttack == null)
        {
            return;
        }
        GameObject target = GetComponent<Attackcontroller>().targettoAttack.gameObject;

        if (animator.GetBool("SpecialAttack") && target != null)
        {
            Vector3 position = new(target.transform.position.x, 1f, target.transform.position.z);
            Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
            GameObject obj = Instantiate(aoeEffect, position, rotation);

            Collider[] colliders = Physics.OverlapSphere(target.transform.position, 5f);

            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col.CompareTag("Enemy"))
                {
                    GameObject unit = col.gameObject;
                    if (unit != null)
                    {
                        if (unitsAroundTarget.Contains(unit))
                        {
                            continue;
                        }
                        else
                        {
                            unitsAroundTarget.Add(unit);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            for (int i = 0; i < unitsAroundTarget.Count; i++)
            {
                var unit = unitsAroundTarget[i];
                if (unit != null)
                {
                    unit.GetComponent<Unit>().TakeDamage(20);
                }
                else
                {
                    continue;
                }
            }

            animator.SetBool("SpecialAttack", false);
            unitsAroundTarget.Clear();
        }
    }
}
