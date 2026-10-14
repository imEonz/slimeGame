using System;
using UnityEngine;

[Serializable]
public class TriggerBinding
{
    public TriggerType trigger;
    [Range(0, 1)] public float chance = 1f;
    public SkillEffect[] effects;
}