﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Script to manager the control of a player
/// </summary>
public class PlayerController : NetworkBehaviour {

    /// <summary>
    /// Horizontal scale for the displacement
    /// </summary>
    public float m_scaleHorizontal = 150.0f;

    /// <summary>
    /// Vertical scale for the displacement
    /// </summary>
    public float m_scaleVertical = 3.0f;

    /// <summary>
    /// Fields for the bullet
    /// </summary>
    public GameObject m_bulletPrefab;

    /// <summary>
    /// Spawn for the bullets
    /// </summary>
    public Transform m_bulletSpawn;
    
    /// <summary>
    /// Bullet speed
    /// </summary>
    public float m_bulletSpeed = 6.0f;

    /// <summary>
    /// Bullet life time after its creation
    /// </summary>
    public float m_bulletLifeTime = 2.0f;

    /// <summary>
    /// Position to move the camera to the right position to follow the player
    /// </summary>
    public Vector3 m_cameraPosition;
    public Vector3 m_cameraRotation;

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
        {
            return;

        }

        detectInput();

    }    

    /// <summary>
    /// Detects the input and displace the player
    /// </summary>
    void detectInput()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * m_scaleHorizontal;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * m_scaleVertical;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }

    /// <summary>
    /// Network function to fire a bullet (instantiate the prefab then fire it with velocity)
    /// </summary>
    [Command]
    void CmdFire()
    {
        //Create the Bullet from the bullet prefab
        GameObject bullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, m_bulletSpawn.rotation) as GameObject;

        //Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * m_bulletSpeed;

        //Spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        //Destory the bullet after a certain time
        Destroy(bullet, m_bulletLifeTime);
    }

    /// <summary>
    /// Override: Behavior only done by the local player at the start
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;

        Camera.main.transform.SetParent(gameObject.transform);
        Camera.main.transform.localPosition = m_cameraPosition;
        Camera.main.transform.localEulerAngles = m_cameraPosition;
    }
}
