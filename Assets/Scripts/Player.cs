﻿using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _playerspeed = 6.0f;
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
    private bool _shieldactive = false;
    private bool _speedactive = false;
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
    [SerializeField]
    private float _shotactivetime;
    private bool _shotpowerupactive = false;
    [SerializeField]
    private float _turbohealth = 3.0f;
    private bool _turboactive = false;
    private Camera _camera;
    [SerializeField]
    private int _shotType = 0;  //  0 = normal  1 = tripleshot  2 = wavelaser 3 = triplewaveshot
    [SerializeField]
    private GameObject _wavelaserPrefab = default;
    [SerializeField]
    private GameObject _triplewaveshotPrefab = default;
    [SerializeField]
    private bool _powerDownActive = false;
    [SerializeField]
    private int _storeAmmoCount;
    [SerializeField]
    private float _powerDownActiveTime;
    

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
        _camera = FindObjectOfType<Camera>();
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
        if (_turbohealth < 3f && _turboactive == false)
        {
            TurboRecharge();
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
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -9.5f, 11.5f), 0);

        if (transform.position.x >= 19.5f)
        {
            transform.position = new Vector3(-19.5f, transform.position.y, 0);
        }
        else if (transform.position.x <= -19.5)
        {
            transform.position = new Vector3(19.5f, transform.position.y, 0);
        }
    }

    private void EngageTurbo()
    {
        _turboactive = true;
        if (_turbohealth > 0)
        {
            _turbo = 3.0f;
            _turbohealth -= Time.deltaTime;
            _uiManager.BoostMeter(_turbohealth);
            if (_turbohealth <= 0)
            {
                _turbohealth = 0;
                _turbo = 0;
            }
        }
    }

    private void DisengageTurbo()
    {
        _turbo = 0f;
        _turboactive = false;
    }

    private void TurboRecharge()
    {
        _turbohealth += Time.deltaTime / 3;
        _uiManager.BoostMeter(_turbohealth);
        if (_turbohealth > 3f)
        {
            _turbohealth = 3f;
            _uiManager.BoostMeter(_turbohealth);
        }
    }

    private void LaserFire()
    {
        Vector3 laserOffset = new Vector3(0, 1.02f, 0);
        
        switch (_shotType)
        {
            case 0:
                if (_ammocount > 0)
                {
                    Instantiate(_laserPrefab, transform.position + laserOffset, Quaternion.identity);
                    _canfire = Time.time + _fireRate;
                    _lasersource.PlayOneShot(_pewpew, 1.0f);
                    _ammocount--;
                    if (_uiManager != null)
                    {
                        _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
                    }
                }
                else if (_ammocount == 0)
                {
                    _emptysource.PlayOneShot(_emptyclick, 1.0f);
                }
                return;

            case 1:
                Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
                _canfire = Time.time + _fireRate;
                _lasersource.PlayOneShot(_pewpew, 1.0f);
                return;

            case 2:
                Instantiate(_wavelaserPrefab, transform.position + laserOffset, Quaternion.identity);
                _canfire = Time.time + _fireRate;
                _lasersource.PlayOneShot(_pewpew, 1.0f);
                return;

            case 3:
                Instantiate(_triplewaveshotPrefab, transform.position + laserOffset, Quaternion.identity);
                _canfire = Time.time + _fireRate;
                _lasersource.PlayOneShot(_pewpew, 1.0f);
                return;

            default:
                Debug.LogError("_shotType defaulted");
                return;
        }


    }
    
    public void HealthManagement(bool _heal)
    {
        if (_shieldactive == true && _heal == false)
        {
            ShieldDamage();
            return;
        }
        if (_heal == false)
        {
            _lives--;
            StartCoroutine(_camera.CameraShake(2));
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
                GameObject _boom = Instantiate(_playerExplosionPrefab, transform.position, Quaternion.identity);
                Destroy(_boom, 2.35f);
                Destroy(this.gameObject);
                if (_spawnManager != null)
                {
                    _spawnManager.OnPlayerDeath();
                }
                Enemy[] _enemy = FindObjectsOfType<Enemy>();
                for (int i = 0; i < _enemy.Length; i++)
                {
                    _enemy[i].OnPlayerDeath();
                }
                Enemy_Minelayer[] _enemyMinelayer = FindObjectsOfType<Enemy_Minelayer>();
                for (int i = 0; i < _enemyMinelayer.Length; i++)
                {
                    _enemyMinelayer[i].OnPlayerDeath();
                }
                Mine[] _mine = FindObjectsOfType<Mine>();
                for (int i = 0; i < _mine.Length; i++)
                {
                    _mine[i].OnPlayerDeath();
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
            default:
                Debug.LogError("_lives defaulted");
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
        if (_shotType == 2)
        {
            Synergy();
            return;
        }
        _shotpowerupactive = true;
        _shotType = 1;
        _shotactivetime += 5.0f;
        StartCoroutine(PowerupPowerDownRoutine());
    }
    public void SpeedActive()
    {
        _speedactive = true;
        _playerspeedboost = 3.0f;
        _speedactivetime += 5.0f;
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

    public void WaveShotActive()
    {
        if (_shotType == 1)
        {
            Synergy();
            return;
        }
        _shotpowerupactive = true;
        _shotType = 2;
        _shotactivetime += 5.0f;
        StartCoroutine(PowerupPowerDownRoutine());
    }

    private void Synergy()
    {
        _shotpowerupactive = true;
        _shotType = 3;
        _shotactivetime = 10f;
    }

    public void PowerDown()
    {
        _storeAmmoCount = _ammocount;
        _ammocount = 0;
        _playerspeedboost = -3f;
        _powerDownActive = true;
        _powerDownActiveTime += Random.Range(1f, 10f);
        _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
        StartCoroutine(PowerDownPowerDownRoutine());
    }

    IEnumerator PowerDownPowerDownRoutine()
    {
        while (_powerDownActive == true)
        {
            yield return new WaitForSeconds(_powerDownActiveTime);
            _powerDownActiveTime = 0;
            if (_ammocount == 0)
            {
                _ammocount = _storeAmmoCount;
            }
            _playerspeedboost = 0f;
            _powerDownActive = false;
            _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
        }
        yield return null;
    }


    public void Reload()
    {
        _ammocount = 15;
        _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
    }

    IEnumerator PowerupPowerDownRoutine()
    {
            while (_shotpowerupactive == true | _speedactive == true)
            {
                if (_shotpowerupactive == true)
                {
                    _shotactivetime -= Time.deltaTime;
                _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
                if (_shotactivetime <= 0)
                    {
                        _shotactivetime = 0;
                        _shotType = 0;
                        _shotpowerupactive = false;
                    }
                }
                if (_speedactive == true)
                {
                    _speedactivetime -= Time.deltaTime;
                    if (_speedactivetime <= 0)
                    {
                        _speedactivetime = 0;
                        _playerspeedboost = 0;
                        _speedactive = false;
                    }
                }
            _uiManager.UpdateAmmo(_ammocount, _shotpowerupactive, _shotactivetime);
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
