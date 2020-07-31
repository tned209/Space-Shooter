using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _playerspeed = 5.0f;
    private float _playerspeedboost = 0f;
    [SerializeField]
    private GameObject _laserPrefab = default;
    [SerializeField]
    private GameObject _tripleshotPrefab = default;
    private float _fireRate = 0.20f;
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
    private GameObject _explosionRef = default;
    private AudioSource _playerexplosionsfx;
    private AudioClip _playerboom;
    private float _turbo = 0f;
    private int _shieldstrength = 0;
    private SpriteRenderer _shieldRenderer = default;
    private int _ammocount = 15;
    private GameObject _emptyRef = default;
    [SerializeField]
    private AudioSource _emptysource = default;
    private AudioClip _emptyclick = default;
   
    
    // Start is called before the first frame update
    void Start()
    {
        //reset player position to 0,-3.5,0
        transform.position = new Vector3(0.0f, -3.5f, 0.0f);
        _uiManager = GameObject.FindObjectOfType<UIManager>();
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _pewpew = _lasersource.clip;
        _explosionRef = GameObject.Find("explosion_sound");
        _playerexplosionsfx = _explosionRef.GetComponent<AudioSource>();
        _playerboom = _playerexplosionsfx.clip;
        _shieldRenderer = _activeshieldvisualizer.GetComponent<SpriteRenderer>();
        if (null == _shieldRenderer)
        {
            Debug.LogError("Player _shieldrenderer is NULL");
        }
        _emptyRef = GameObject.Find("no_ammo");
        _emptysource = _emptyRef.GetComponent<AudioSource>();
        _emptyclick = _emptysource.clip;
        if (null == _spawnManager)
        {
            Debug.LogError("Player Spawn Manager is NULL");
        }
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            EngageTurbo();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            DisengageTurbo();
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

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * (_playerspeed + _playerspeedboost + _turbo) * Time.deltaTime);

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

    private void EngageTurbo()
    {
        _turbo = 3.0f;
    }

    private void DisengageTurbo()
    {
        _turbo = 0f;
    }

    private void LaserFire()
    {
        Vector3 laserOffset = new Vector3(0, 1.02f, 0);
        if (_tripleshotactive == false)
        {
            if (_ammocount > 0)
            {
                Instantiate(_laserPrefab, transform.position + laserOffset, Quaternion.identity);
                _canfire = Time.time + _fireRate;
                _lasersource.PlayOneShot(_pewpew, 1.0f);
                _ammocount--;
                if(_uiManager != null)
                {
                    _uiManager.UpdateAmmo(_ammocount, _tripleshotactive, _tripleshotactivetime);
                }
            }
            else if (_ammocount == 0)
            {
                _emptysource.PlayOneShot(_emptyclick, 1.0f);
            }
        }
        else if (_tripleshotactive == true)
        {
            Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
            _lasersource.PlayOneShot(_pewpew, 1.0f);
        }

    }
    public void HealthManagement(bool _heal)
    {
        Debug.Log("_heal = " + _heal);
        if (_shieldactive == true && _heal == false)
        {
            ShieldDamage();
            return;
        }
        if (_heal == false)
        {
            _lives--;
        }
        else if (_heal == true && _lives < 3)
        {
            _lives++;
        }
        _uiManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 0:
                _playerexplosionsfx.PlayOneShot(_playerboom, 1.0f);
                Instantiate(_playerExplosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                if (_spawnManager != null)
                {
                    _spawnManager.OnPlayerDeath();
                }
                return;
            case 1:
                _rightdamagevisualizer.SetActive(true);
                _leftdamagevisualizer.SetActive(true);
                return;
            case 2:
                _leftdamagevisualizer.SetActive(true);
                _rightdamagevisualizer.SetActive(false);
                return;
            case 3:
                _leftdamagevisualizer.SetActive(false);
                _rightdamagevisualizer.SetActive(false);
                return;
        }
    }

    private void ShieldDamage()
    {
        if (_shieldstrength > 0)
        {
            _shieldstrength--;
        }
        if (_shieldstrength == 0)
        {
            _shieldactive = false;
            _activeshieldvisualizer.SetActive(false);
        }
        ShieldStrengthVisualizerController();
    }
    private void ShieldStrengthVisualizerController()
    {
        switch (_shieldstrength)
        {
            case 0:
                break;
            case 1:
                _activeshieldvisualizer.transform.localScale = new Vector3(1.7f, 1.7f, 0);
                _shieldRenderer.color = new Color32(255, 255, 255, 75);
                break;
            case 2:
                _activeshieldvisualizer.transform.localScale = new Vector3(1.9f, 1.9f, 0);
                _shieldRenderer.color = new Color32(255, 255, 255, 150);
                break;
            case 3:
                _activeshieldvisualizer.transform.localScale = new Vector3(2.1f, 2.1f, 0);
                _shieldRenderer.color = new Color32(255, 255, 255, 255);
                break;
            default:
                Debug.LogError("Player _shieldstrength defaulted");
                break;
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
        _playerspeedboost = 3.0f;
        _speedactive = true;
        _speedactivetime = _speedactivetime + 5.0f;
        StartCoroutine(PowerupPowerDownRoutine());
    }
    public void ShieldActive()
    {
        if (_shieldstrength < 3)
        {
            _shieldstrength++;
        }
        ShieldStrengthVisualizerController();
        _activeshieldvisualizer.SetActive(true);
        _shieldactive = true;
    }

    public void Reload()
    {
        _ammocount = 15;
        _uiManager.UpdateAmmo(_ammocount, _tripleshotactive, _tripleshotactivetime);
    }

    IEnumerator PowerupPowerDownRoutine()
    {
        while (_tripleshotactive == true | _speedactive == true)
        {
            if (_tripleshotactive == true)
            {
                _uiManager.UpdateAmmo(_ammocount, _tripleshotactive, _tripleshotactivetime);
                _tripleshotactivetime -= Time.deltaTime;
                if (_tripleshotactivetime <= 0)
                {
                    _tripleshotactivetime = 0;
                    _tripleshotactive = false;
                    _uiManager.UpdateAmmo(_ammocount, _tripleshotactive, _tripleshotactivetime);
                }
            }
            if (_speedactive == true)
            {
                _speedactivetime -= Time.deltaTime;
                if (_speedactivetime <= 0)
                {
                    _speedactivetime = 0;
                    _speedactive = false;
                    _playerspeedboost = 0f;
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
}
