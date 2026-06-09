
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Circle", menuName = "ScriptableObjects/Circle", order = 1) ]
public class Circle : ScriptableObject
{
    [SerializeField]
    private string Name;

    [SerializeField]
    public Circle next_Circle;

    [SerializeField]
    public UnityEngine.Object next_circle;

}