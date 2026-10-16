public interface IWeaponController
{
    void SetDamage(float damage);
    void SetFireRate(float firerate);
    // void ApplyAffix();
    // void SetSprite(SpriteRenderer sprite);

    // this all seems so useless now since it just goes into the guncontroller which then passes down to the weapons so its like ??? why even have it
}

