using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimpleScript : MonoBehaviour
{
    public float movementSpeed = 2f;
    public GameObject bulletPrefab;
    public float reloadTime = 0.5f;

    private float weaponHeight;
    private float currentMovementSpeed;
    private float prevShotTime;

    // Start is called before the first frame update
    void Start()
    {
        weaponHeight = GetComponent<MeshRenderer>().bounds.size.y / 2.0f;
        currentMovementSpeed = movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        var aim = GetAimPosition();
        RotateTo(aim);
        if (Input.GetMouseButton(0))
        {
            Shoot(aim);
        }
    }

    private void Move()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement = Vector2.ClampMagnitude(movement, 1f);
        // смещение
        Vector3 offset = new Vector3(movement.x, 0.0f, movement.y) * currentMovementSpeed;

        // игрок двигается в направлении курсора
        //transform.Translate(offset.normalized * currentMovementSpeed * Time.deltaTime);

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f)
            transform.position += offset * Time.deltaTime;
    }

    private Vector3 GetAimPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        Vector3 result;
        if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            result = new Vector3(hit.point.x, weaponHeight, hit.point.z);
        } else
        {
            result = transform.position;
        }
        return result;
    }

    private void RotateTo(Vector3 aim)
    {
        transform.LookAt(aim, Vector3.up);
        //Debug.DrawLine(aim, new Vector3(aim.x, 2, aim.z));
    }

    private void Shoot(Vector3 aim)
    {
        
        if (Time.time - prevShotTime >= reloadTime)
        {
            var rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 90.0f, bulletPrefab.transform.rotation.eulerAngles.z);
            var bullet = Instantiate(bulletPrefab, transform.position, rotation);
            var bulletController = bullet.GetComponent<IBulletController>();
            //bulletController.SetDirection(Vector3.Normalize(aim - transform.position));
            //bulletController.SetPlayer(gameObject);
            bulletController.SetVelocity(Vector3.Normalize(aim - transform.position) * 10f);
            bulletController.SetShooter(gameObject);
            prevShotTime = Time.time;
        }
        
    }
}
