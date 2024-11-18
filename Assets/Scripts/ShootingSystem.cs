using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] float _layerZoom;
    [SerializeField] float _speedZoom;
    [SerializeField] Animator _anim;
    public bool _isZooming;
    Transform[] EnemyesTransform;
    public int _damage = 25;
    public float _distanceAttackSword = 2f;

    private void Start()
    {
        SetEnemyesArray();
    }

    void Update()
    {
        if (_isZooming && _layerZoom < 1f)
            _layerZoom += _speedZoom * Time.deltaTime;
        else if (!_isZooming && _layerZoom > 0f)
            _layerZoom -= _speedZoom * Time.deltaTime;

        _anim.SetLayerWeight(1, _layerZoom);

        if (Vector3.Distance(transform.position, GetComponent<ShootingSystem>().GetNearPos()) <= _distanceAttackSword)
            _anim.SetBool("AttackSword", true);
        else
            _anim.SetBool("AttackSword", false);
    }

    public Vector3 GetNearPos()
    {
        Vector3 nearPos = new Vector3(0f, 1000f, 0f);
        for (int i = 0; i < EnemyesTransform.Length; i++)
        {
            if (EnemyesTransform[i] && Vector3.Distance(transform.position, EnemyesTransform[i].position) < Vector3.Distance(transform.position, nearPos))
                nearPos = EnemyesTransform[i].position;
        }
        return nearPos;
    }

    public void GiveDamage()
    {
        for (int i = 0; i < EnemyesTransform.Length; i++)
        {
            if (EnemyesTransform[i] && Vector3.Distance(transform.position, EnemyesTransform[i].position) <= _distanceAttackSword)
                EnemyesTransform[i].GetComponent<EnemyScript>().TakeDamage(_damage);
        }
    }

    public void SetEnemyesArray()
    {
        GameObject[] EnemyesObj;
        EnemyesObj = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyesTransform = new Transform[EnemyesObj.Length];
        for (int i = 0; i < EnemyesObj.Length; i++)
        {
            EnemyesTransform[i] = EnemyesObj[i].transform;
        }
    }

}
