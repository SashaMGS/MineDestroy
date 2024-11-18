using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int _hp = 100;
    private GameObject _player => GameObject.FindGameObjectWithTag("Player");

    public void TakeDamage(int damage)
    {
        if (_hp > 0)
            _hp -= damage;
        if (_hp <= 0)
        {
            Destroy(gameObject);
            _player.GetComponent<ShootingSystem>().SetEnemyesArray();
        }
    }
}
