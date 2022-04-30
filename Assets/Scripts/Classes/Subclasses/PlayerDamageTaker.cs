using System.Collections.Generic;

public class PlayerDamageTaker : DamageTaker
{
    private void Update()
    {
        EventManager.TriggerEvent("currentHealthPlayer", new Dictionary<string, object> { { "amount", this.FracHealth } });
    }

    internal override void DoDeath()
    {
        // TODO: Switch scenes using SceneManager
        return;
    }
}