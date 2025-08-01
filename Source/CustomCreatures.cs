using System;

namespace Shapeshift.Source;

[Serializable]
public class CustomCreature
{
    public string PrefabName = "";
    public string OverrideName = "";
    public string PrimaryAttack = "";
    public string SecondaryAttack = "";
    public string BlockAttack = "";
    public bool UsePlayerAnimator;
    public bool HideWeapons = true;
    public bool HideArmor = true;
    public bool HideHelmet = true;
    public bool RequireWeaponEquipped = false;
    public bool RequireRangedWeaponEquipped = false;
    public bool WaterCreature = false;
    public bool FlyingCreature = false;
    public float CameraMaxDistance = 8f;
    public string HeadTransform = "Head";
    public bool EnableUseItem = false;
    public string UseItem = "";
    public float Duration = 600f;
}