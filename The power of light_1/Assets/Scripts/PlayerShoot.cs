using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //ЛКМ
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //Позиция мыши(куда наводится мышь, туда и будет стрелять)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        Destroy(bullet, 2f);
        SoundEffectManager.Play("Bullet");
    }
}
