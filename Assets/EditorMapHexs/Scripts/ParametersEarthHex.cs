using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersEarthHex : ScriptableObject
{
    public Texture2D TextureHexBase => textureHexBase;
    [SerializeField] private Texture2D textureHexBase = null;
    public Texture2D TextureHexPointer => textureHexPointer;
    [SerializeField] private Texture2D textureHexPointer = null;
    public Color ColorArrow => colorArrow;
    [SerializeField] private Color colorArrow = Color.white;
}
