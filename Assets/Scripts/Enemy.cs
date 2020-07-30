using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float _enemyspeed = 4.0f;
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
        //respawn enemy up top if it makes it to the bottom of the field alive, otherwise propel it towards the bottom
        if (transform.position.y >= -5.75f)
        {
            transform.Translate(new Vector3(0, -1 * _enemyspeed * Time.deltaTime, 0));
        }
        else if (transform.position.y <= -5.75f && _name != "Dying Enemy")
        {
            transform.position = new Vector3(Random.Range(-9.5f, 9.5f), 5.75f, 0);
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
            _player.TrackScore(_scorevalue);
            if (_player != null)
            {
                _player.Damage();
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
            _player.TrackScore(_scorevalue);
        }
    }

    private void LaserFire()
    {
        Vector3 laserOffset = new Vector3(0, -1.02f, 0);
        _lasersource.PlayOneShot(_pewpew, 1.0f);
        Instantiate(_enemyLaserPrefab, transform.position + laserOffset, Quaternion.identity);
        _refire = Time.time + Random.Range(3.0f, 7.0f);
    }
}
    
