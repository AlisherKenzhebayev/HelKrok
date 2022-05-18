using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Inventory System/Ability/Default Projectile Ability")]
public class DefaultProjectileAbilityObject : BaseAbilityItemObject
{
    public GameObject prefabToShoot;
    public float timerToSpawn = 2f;
    public float numberToSpawn = 2f;
    public bool loop = false;
    public float timerCooldown = 2f;

    private ProjectileSpawner projectileSpawner = null;

    public override void Awake()
    {
        base.Awake();
        tagName = "projectile";
    }

    public override void Execute(GameObject go, bool enable) 
    {
        if (projectileSpawner == null)
        {
            projectileSpawner = go.GetComponent<ProjectileSpawner>();
            if (projectileSpawner == null) {
                projectileSpawner = go.AddComponent<ProjectileSpawner>();
            }
        }

        projectileSpawner.projectileToSpawn = prefabToShoot;
        projectileSpawner.timerToSpawn = timerToSpawn;
        projectileSpawner.numberToSpawn = numberToSpawn;
        projectileSpawner.loop = loop;
        projectileSpawner.timerCooldown = timerCooldown;

        projectileSpawner.StartSpawning();// (enable ? true : false);
    }
}
