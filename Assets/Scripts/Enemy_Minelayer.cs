using System.Collections;
using UnityEngine;

public class Enemy_Minelayer: MonoBehaviour
{
    [SerializeField]
    float _speed = 6.0f;
    private int _scorevalue = 5000;
    private Player _player;
    private BoxCollider2D _bc;
    private string _name;
    private GameObject ExplosionRef;
    private AudioSource _enemyexplosionsfx;
    private AudioClip _enemyboom;
    [SerializeField]
    private GameObject _minePrefab = default;
    private float _refire = default;
    private AudioClip _mineDropClip = default;
    private AudioSource _mineDropSource = default;
    private GameObject _mineDropRef = default;
    [SerializeField]
    private bool _playeralive = true;
    private Vector3 _spawnloc;
    private int _health = 2;
    [SerializeField]
    private GameObject _explosionAnimation = default;
    private SpriteRenderer _sr;
    private GameObject _impactRef = default;
    private AudioSource _impactSource = default;
    private AudioClip _impactClip = default;
    [SerializeField]
    private bool _movingright = false;
    SpawnManager _spawnManager;


    // Start is called before the first frame update
    void Start()
    {
        _name = gameObject.name;
        _player = FindObjectOfType<Player>();
        _bc = gameObject.GetComponent<BoxCollider2D>();
        ExplosionRef = GameObject.Find("explosion_sound");
        _enemyexplosionsfx = ExplosionRef.GetComponent<AudioSource>();
        _enemyboom = _enemyexplosionsfx.clip;
        _mineDropRef = GameObject.Find("mine_drop");
        _mineDropSource = _mineDropRef.GetComponent<AudioSource>();
        _mineDropClip = _mineDropSource.clip;
        _refire = Time.time + Random.Range(1.0f, 4.0f);
        _spawnloc = transform.position;
        if (_spawnloc.x == -19.4f)
        {
            _movingright = true;
        }
        else if (_spawnloc.x == 19.4f)
        {
            _movingright = false;
        }
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _impactRef = GameObject.Find("weapon_impact");
        _impactSource = _impactRef.GetComponent<AudioSource>();
        _impactClip = _impactSource.clip;
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //this enemy spawns at the horizontal edge of the playfield and moves side to side
        Movement();

        //enemy drops mines frequently
        if (_refire <= Time.time && _name != "Dying Enemy")
        {
            MineDrop();
        }

    }

    private void Movement()
    {
        if (transform.position.x < 19.5f && _movingright == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(-19.5f, transform.position.y, 0), _speed * Time.deltaTime);
        }
        if (transform.position.x <= -19.4)
        {
            _movingright = true;
        }
        if (transform.position.x > -19.5f && _movingright == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(19.5f, transform.position.y, 0), _speed * Time.deltaTime);
        }
        if (transform.position.x >= 19.4)
        {
            _movingright = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_health > 1)
            {
                _health--;
                _player.HealthManagement(false);
                _impactSource.PlayOneShot(_impactClip, 1.0f);
                return;
            }
            else if (_health <= 1)
            {
                Death();
                if (_player != null)
                {
                    _player.HealthManagement(false);
                    _player.TrackScore(_scorevalue);
                }
            }
        }
            
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_health > 1)
            {
                _health--;
                _impactSource.PlayOneShot(_impactClip, 1.0f);
                return;
            }
            else if (_health == 1)
            {
                Death();
                if (_player != null)
                {
                    _player.TrackScore(_scorevalue);
                }
            }
        }
    }

    private void Death()
    {
        _name = ("Dying Enemy");
        Destroy(_bc);
        _enemyexplosionsfx.PlayOneShot(_enemyboom, 1.0f);
        GameObject _newExplosion = Instantiate(_explosionAnimation, transform.position, Quaternion.identity);
        _newExplosion.transform.parent = gameObject.transform;
        StartCoroutine(RenderDelay());
        Destroy(this.gameObject, 1.36f);
        _spawnManager.KillTracker();
    }

        private void MineDrop()
    {
        if (_playeralive == true)
        {
            Vector3 _mineoffset = new Vector3(0, -1.27f, 0);
            _mineDropSource.PlayOneShot(_mineDropClip, 1.0f);
            Instantiate(_minePrefab, transform.position + _mineoffset, Quaternion.identity);
            _refire = Time.time + Random.Range(1.5f, 3.0f);
        }
    }

    public void OnPlayerDeath()
    {
        _playeralive = false;
    }

    IEnumerator RenderDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(_sr);
        yield return null;
    }
}