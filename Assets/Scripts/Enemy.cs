//using System.Numerics;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float _enemyspeed = 5.0f;
    private int _scorevalue = 1000;
    private Animator _animator;
    private Player _player;
    private BoxCollider2D _bc;
    private string _name;
    private GameObject ExplosionRef;
    private AudioSource _enemyexplosionsfx;
    private AudioClip _enemyboom;
    [SerializeField]
    private GameObject _enemyLaserPrefab = default;
    private float _refire = default;
    private AudioClip _pewpew = default;
    private AudioSource _lasersource = default;
    private GameObject LaserRef = default;
    [SerializeField]
    private bool _playeralive = true;
    private bool _shotfired = false;
    private bool _adjustPosition = false;
    [SerializeField]
    private Vector3 _targetPos = default;


    // Start is called before the first frame update
    void Start()
    {
        _name = this.gameObject.name;
        _animator = this.gameObject.GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _bc = this.gameObject.GetComponent<BoxCollider2D>();
        ExplosionRef = GameObject.Find("explosion_sound");
        _enemyexplosionsfx = ExplosionRef.GetComponent<AudioSource>();
        _enemyboom = _enemyexplosionsfx.clip;
        LaserRef = GameObject.Find("laser_shot");
        _lasersource = LaserRef.GetComponent<AudioSource>();
        _pewpew = _lasersource.clip;
        _refire = Time.time + Random.Range(1.0f, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_shotfired == true)
        {
            float _shift = 0;

            if (Random.value >= 0.33f)
            {
                _shift = 0;
            }
            else if (Random.value >= 0.5f)
            {
                _shift = -2f;
            }
            else _shift = 2f;
            _targetPos = new Vector3(transform.position.x + _shift, transform.position.y - 2f, 0);
            if (_targetPos.x < -19.5f)
            {
                _targetPos.x = -19.5f;
            }
            if (_targetPos.x > 19.5)
            {
                _targetPos.x = 19.5f;
            }
            _shotfired = false;
            _adjustPosition = true;
        }

        if (_adjustPosition == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _enemyspeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPos) < .01f)
            {
                _adjustPosition = false;
            }
        }
        

        //respawn enemy up top if it makes it to the bottom of the field alive, otherwise propel it towards the bottom, allowing it to shift position if it has fired recently
        if (transform.position.y >= -9.5f && _adjustPosition == false)
        {
            transform.Translate(new Vector3(0, -1 * _enemyspeed, 0) * Time.deltaTime);
        }
        else if (transform.position.y <= -9.5f && _name != "Dying Enemy")
        {
            transform.position = new Vector3(Random.Range(-19.5f, 19.5f), 11.5f, 0);
            _adjustPosition = false;
        }

        //enemy fires laser occasionally
        if (_refire <= Time.time && _name != "Dying Enemy")
        {
            LaserFire();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _name = ("Dying Enemy");
            Destroy(_bc);
            _enemyexplosionsfx.PlayOneShot(_enemyboom, 1.0f);
            _animator.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.8f);
            if (_player != null)
            {
                _player.HealthManagement(false);
                _player.TrackScore(_scorevalue);
            }
        }
        else if (other.tag == "Laser")
        {
            _name = ("Dying Enemy");
            Destroy(other.gameObject);
            Destroy(_bc);
            _enemyexplosionsfx.PlayOneShot(_enemyboom, 1.0f);
            _animator.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.8f);
            if (_player != null)
            {
                _player.TrackScore(_scorevalue);
            }
        }
    }

    public void Death()
    {
        _name = ("Dying Enemy");
        Destroy(_bc);
        _enemyexplosionsfx.PlayOneShot(_enemyboom, 1.0f);
        _animator.SetTrigger("OnEnemyDeath");
        Destroy(this.gameObject, 2.8f);
        if (_player != null)
        {
            _player.TrackScore(_scorevalue);
        }
    }

    private void LaserFire()
    {
        if (_playeralive == true)
        {
            _shotfired = true;
            Vector3 laserOffset = new Vector3(0, -1.02f, 0);
            _lasersource.PlayOneShot(_pewpew, 1.0f);
            Instantiate(_enemyLaserPrefab, transform.position + laserOffset, Quaternion.identity);
            _refire = Time.time + Random.Range(3.0f, 7.0f);
        }
    }

    public void OnPlayerDeath()
    {
        _playeralive = false;
    }
}
    
