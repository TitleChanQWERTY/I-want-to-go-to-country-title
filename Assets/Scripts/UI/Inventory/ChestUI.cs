using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    private bool BlockerInputSystem() => true;

    private void OnEnable()
    {
        PlayerInputSystem.BlockInputSystem += BlockerInputSystem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.BlockInputSystem -= BlockerInputSystem;
    }
}
