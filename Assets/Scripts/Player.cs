using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _playerspeed = 7.0f;
    private float _playerspeedboost = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab = default;
    [SerializeField]
    private GameObject _tripleshotPrefab = default;
    private float _fireRate = 0.15f;
    private float _canfire = -1.0f;
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    [SerializeField]
    private bool _tripleshotactive = false;
    [SerializeField]
    private bool _speedactive = false;
    [SerializeField]
    private bool _shieldactive = false;
    private float _tripleshotactivetime;
    private float _speedactivetime;
    [SerializeField]
    private GameObject _activeshieldvisualizer = default;
    private int _score = 0;
    [SerializeField]
    private GameObject _thrustervisualizer = default;
    [SerializeField]
    private GameObject _leftdamagevisualizer = default;
    [SerializeField]
    private GameObject _rightdamagevisualizer = default;
    private AudioClip _pewpew = default;
    [SerializeField]
    private AudioSource _lasersource = default;
    [SerializeField]
    private GameObject _playerExplosionPrefab = default;
    private GameObject ExplosionRef;
    private AudioSource _playerexplosionsfx;
    private AudioClip _playerboom;




    // Start is called before the first frame update
    void Start()
    {
        //reset player position to 0,-3.5,0
        transform.position = new Vector3(0.0f, -3.5f, 0.0f);
        _uiManager = GameObject.FindObjectOfType<UIManager>();
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _pewpew = _lasersource.clip;
        ExplosionRef = GameObject.Find("explosion_sound");
        _playerexplosionsfx = ExplosionRef.GetComponent<AudioSource>();
        _playerboom = _playerexplosionsfx.clip;



    }
    // Update is called once per frame
    void Update()
    {
        
        CalculateMovement();
        //set spacebar to fire laser and check refire rate
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canfire)
        {
            LaserFire();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            _thrustervisualizer.SetActive(true);
        }
        else
        {
            _thrustervisualizer.SetActive(false);
        }

        //move player at adjustable speed in real time with controller inputs
        if (_speedactive == true)
        {
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * (_playerspeed + _playerspeedboost) * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _playerspeed * Time.deltaTime);
        }
        
        // restricting player to playspace with horizontal wraparound
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 5.8f), 0);

        if (transform.position.x >= 9.5f)
        {
            transform.position = new Vector3(-9.5f, transform.position.y, 0);
        }
        else if (transform.position.x <= -9.5)
        {
            transform.position = new Vector3(9.5f, transform.position.y, 0);
        }
    }
    void LaserFire()
    {
        //fire laser and reset time last fired
        Vector3 laserOffset = new Vector3(0, 1.02f, 0);
        _canfire = Time.time + _fireRate;
        _lasersource.PlayOneShot(_pewpew, 1.0f);
        if (_tripleshotactive == false)
        {
            Instantiate(_laserPrefab, transform.position + laserOffset, Quaternion.identity);
        }
        else
        {
            Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
        }
        
    }
    public void Damage()
    {
        if(_shieldactive == true)
        {
            _shieldactive = false;
            _activeshieldvisualizer.SetActive(false);
            return;
        }
        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _leftdamagevisualizer.SetActive(true);
        }

        if (_lives == 1)
        {
            _rightdamagevisualizer.SetActive(true);
        }

        if (_lives <= 0)
        {
            if (_spawnManager == null)
            {
                Debug.LogError("The Spawn Manager is NULL");
            }
            _playerexplosionsfx.PlayOneShot(_playerboom, 1.0f);
            Instantiate(_playerExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            _spawnManager.OnPlayerDeath();
        }


    }
    public void TripleShotActive()
    {
        _tripleshotactive = true;
        _tripleshotactivetime = _tripleshotactivetime + 5.0f;
        StartCoroutine(PowerupPowerDownRoutine());
    }
    public void SpeedActive()
    {
        _speedactive = true;
        _speedactivetime = _speedactivetime + 5.0f;
        StartCoroutine(PowerupPowerDownRoutine());
    }
    public void ShieldActive()
    {
        _shieldactive = true;
        _activeshieldvisualizer.SetActive(true);
    }

    IEnumerator PowerupPowerDownRoutine()
    {
        while (_tripleshotactive == true | _speedactive == true)
        {
            if (_tripleshotactive == true)
            {
                _tripleshotactivetime -= Time.deltaTime;
                if (_tripleshotactivetime <= 0)
                {
                    _tripleshotactivetime = 0;
                    _tripleshotactive = false;
                }
            }
            if (_speedactive == true)
            {
                _speedactivetime -= Time.deltaTime;
                if (_speedactivetime <= 0)
                {
                    _speedactivetime = 0;
                    _speedactive = false;
                }
            }
            yield return null;
        }
    }
    public void TrackScore(int _scoreupdate)
    {
        _score += _scoreupdate;
        if (_uiManager != null)
        {
            _uiManager.UpdateUIScore(_score);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy_Laser")
        {
            Damage();
        }
    }
}
