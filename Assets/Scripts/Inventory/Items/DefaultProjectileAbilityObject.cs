using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Inventory System/Ability/Default Projectile Ability")]
public class DefaultProjectileAbilityObject : BaseAbilityItemObject
{
    public GameObject prefabToShoot;
    public float timerToSpawn = 2f;
    public float numberToSpawn = 2f;
    public bool loop = false;

    private ProjectileSpawner projectileSpawner = null;

    internal override void Awake()
    {
        base.Awake();
        tagName = "projectile";
    }

    public override bool Execute(GameObject _gameObject, bool _enable, Transform _transform) 
    {
        if (projectileSpawner == null)
        {
            projectileSpawner = _gameObject.GetComponent<ProjectileSpawner>();
            if (projectileSpawner == null) {
                projectileSpawner = _gameObject.AddComponent<ProjectileSpawner>();
            }
        }

        projectileSpawner.projectileToSpawn = prefabToShoot;
        projectileSpawner.timerToSpawn = timerToSpawn;
        projectileSpawner.numberToSpawn = numberToSpawn;
        projectileSpawner.loop = loop;
        projectileSpawner.timerCooldown = timerCooldown;
        projectileSpawner.spawnTransform = _transform;
        
        // TODO: horrible mess
        projectileSpawner.originHitbox = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<DamageTaker>().gameObject;

        if (_enable)
        {
            projectileSpawner.StartFiring();
        }
        else {
            projectileSpawner.StopFiring();
        }

        return true;
    }
}
