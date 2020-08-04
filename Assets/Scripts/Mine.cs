using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    float _normalspeed = 5.0f;
    float _homingspeed = 6.0f;
    private Player _player;
    private Vector3 _playerPos;
    [SerializeField]
    private bool _playerdetected;
    private float _detectionDistance = 5.0f;
    private bool _seeking;
    private bool _playeralive = true;
    [SerializeField]
    private GameObject _explosion = default;
    private Animator _explosionAnim = default;
    private AudioSource _warningBeepSource = default;
    private GameObject _warningBeepRef = default;
    private AudioSource _mineExplosionSource = default;
    private GameObject _mineExplosionpRef = default;

    // Start is called before the first frame update
    void Start()
    {
        _warningBeepRef = GameObject.Find("warning_beep");
        _warningBeepSource = _warningBeepRef.GetComponent<AudioSource>();
        _mineExplosionpRef = GameObject.Find("mine_explosion");
        _mineExplosionSource = _mineExplosionpRef.GetComponent<AudioSource>();
        _explosionAnim = _explosion.GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        if (null == _player)
        {
            Debug.LogError("Mine _player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            _playerPos = _player.transform.position;
        }
        Detection();
        if (_playerdetected == false)
        {
            Movement();
        }
        else Menace();
        SoundFX();
    }

    private void Detection()
    {
        if (Vector3.Distance(_playerPos, transform.position) <= _detectionDistance)
        {
            _playerdetected = true;           
        }
        else
        {
            _playerdetected = false;
        }
    }
    private void Movement()
    {
        transform.Translate(Vector3.down * _normalspeed * Time.deltaTime);
        _seeking = false;
        if (transform.position.y <= -9.5)
        {
            Destroy(gameObject);
        }
    }
    private void Menace()
    {
        if (_playeralive == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerPos, _homingspeed * Time.deltaTime);
            _seeking = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.HealthManagement(false);
            Death();
        }
        if (other.CompareTag("Laser"))
        {
            Death();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            if (_seeking == true)
            {
                Enemy _enemy = other.GetComponent<Enemy>();
                Death();
                _enemy.Death();
            }
        }
    }

    private void SoundFX()
    {
        if (_playeralive == true)
        {
            if (_warningBeepSource.isPlaying == false && _seeking == true)
            {
                _warningBeepSource.Play();
            }
            if (_warningBeepSource.isPlaying == true || _seeking == false)
            {
                return;
            }
        }
    }

    public void Death()
    {
        Animator _newBoom = Instantiate(_explosionAnim, transform.position, Quaternion.identity);
        Destroy(_newBoom.gameObject, 2.0167f);
        _mineExplosionSource.PlayOneShot(_mineExplosionSource.clip, 1.0f);
        Destroy(gameObject);
    }

    public void OnPlayerDeath()
    {
        _playeralive = false;
    }
}