using UnityEngine;


public class PlayerAnimationEvents : MonoBehaviour
{
    private GameObject _player => GameObject.FindGameObjectWithTag("Player");

    public void OnAnimationEvent()
    {
        _player.GetComponent<ShootingSystem>().GiveDamage();
    }
}
