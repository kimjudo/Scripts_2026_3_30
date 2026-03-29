using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("Hit" + objectWeHit.gameObject.name);

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Hit" + objectWeHit.gameObject.name);

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.CompareTag("Zombie"))
        {
            print("Hit" + objectWeHit.gameObject.name);
            Destroy(gameObject);
        }

        void CreateBulletImpactEffect(Collision objectWeHit)
        {
            ContactPoint contact = objectWeHit.contacts[0];

            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectWeHit.transform);
        }

    }
}
