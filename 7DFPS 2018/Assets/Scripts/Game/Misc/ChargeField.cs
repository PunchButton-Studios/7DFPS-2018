using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeField : MonoBehaviour
{
    public float chargeSpeed = 0.01f;

    private void OnTriggerEnter(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
            entityPlayer.isCharging = true;
    }

    private void OnTriggerStay(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
            entityPlayer.energy += chargeSpeed * Time.deltaTime;
    }

    private void OnTriggerExit(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
            entityPlayer.isCharging = false;
    }
}