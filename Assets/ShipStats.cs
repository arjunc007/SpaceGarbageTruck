using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for storing ship stats
public class ShipStats : MonoBehaviour
{
    [SerializeField]
    float acceleration = 1; //Rate of change in ship speed
    float turningSpeed = 2.5f; //Rate at whcih ship turns
    [SerializeField]
    float maxSpeed = 10f;
    float laserStrength = 1;
    float beamLength = 1;
    byte shipStrength = 1;
    bool carryingSalvage = false;
    public int score = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetAcceleration()
    {
        return acceleration;
    }
    public void SetAcceleration(float inAcceleration)
    {
        acceleration += inAcceleration;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetTurningSpeed()
    {
        return turningSpeed;
    }
    public void SetTurningSpeed(float inTurningSpeed)
    {
        turningSpeed += inTurningSpeed;
    }

    public float GetLaserStrength()
    {
        return laserStrength;
    }
    public void SetLaserStrength(float inLaserStrength)
    {
        laserStrength += inLaserStrength;
    }

    public float GetBeamLength()
    {
        return beamLength;
    }
    public void SetBeamLength(float inBeamLength)
    {
        beamLength += inBeamLength;
    }

    public byte GetShipStrength()
    {
        return shipStrength;
    }
    public void SetShipStrength(byte inShipStrength)
    {
        shipStrength += inShipStrength;
    }

    public bool GetSalvage()
    {
        return carryingSalvage;
    }

    public int GetScore()
    {
        return score;
    }
    public void SetScore(int inScore)
    {
        score += inScore;
    }
}
