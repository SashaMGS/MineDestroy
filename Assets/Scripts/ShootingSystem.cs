using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] float _layerZoom;
    [SerializeField] float _speedZoom;
    [SerializeField] Animator _anim;
     public bool _isZooming;
    Transform[] EnemyesTransform;

    private void Start()
    {
        GameObject[] EnemyesObj;
        EnemyesObj = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyesTransform = new Transform[EnemyesObj.Length];
        for (int i = 0; i < EnemyesObj.Length; i++)
        {
            EnemyesTransform[i] = EnemyesObj[i].transform;
        }

    }

    void Update()
    {
        //_isZooming = Input.GetMouseButton(1);

        if (_isZooming && _layerZoom < 1f)
            _layerZoom += _speedZoom * Time.deltaTime;
        else if(!_isZooming && _layerZoom > 0f)
            _layerZoom -= _speedZoom * Time.deltaTime;

        _anim.SetLayerWeight(1, _layerZoom);
    }

    public Vector3 GetNearPos()
    {
        Vector3 nearPos = EnemyesTransform[0].position;
        for (int i = 0; i < EnemyesTransform.Length; i++)
        {
            if (Vector3.Distance(transform.position, EnemyesTransform[i].position) < Vector3.Distance(transform.position, nearPos))
                nearPos = EnemyesTransform[i].position;
        }
        return nearPos;
    }
}
