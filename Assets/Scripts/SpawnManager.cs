using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer = default;
    private bool _playeralive = true;
    [SerializeField]
    private GameObject[] _poweruptype = default;
    [SerializeField]
    private GameObject[] _enemyShips = default;  // 1 = Enemy  2 = MineLayer
    [SerializeField]
    private AnimationCurve _difficultyCurve = default;
    private int _currentWave = 0;
    Dictionary<int, float> _powerups = new Dictionary<int, float>();   // _powerupID from powerupscript, spawn weight
    Dictionary<int, float> _enemySpawn = new Dictionary<int, float>();  // _enemyShips, spawnweight
    private int _enemiesSpawned = 0;
    private int _killsMade = 0;
    private bool _waveComplete = true;
    UIManager _uiManager = default;
    //private float _targetWaveTime = 180;
    private float _spawnRateModifier = 0;
    private int _number0ToSpawn = 0;
    private int _number1ToSpawn = 0;
    private float _balancedMLSpawnTime;
    private float _previousWavePower = 0;
    private float _spawnWeight = 0;


    // Start is called before the first frame update
    void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _enemySpawn.Add(0, 10f);
        _enemySpawn.Add(1, 2f);
        _powerups.Add(0, 5f);
        _powerups.Add(1, 5f);
        _powerups.Add(2, 5f);
        _powerups.Add(3, 15f);
        _powerups.Add(4, 3f);
        _powerups.Add(5, 2f);
        _powerups.Add(6, 1f);
    }

    //Update is called once per frame
    void Update()
    {
        if (_playeralive == false)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if (_waveComplete == true)
        {
            _currentWave++;
            _waveComplete = false;
            StartCoroutine(_uiManager.WaveAnnounce(_currentWave));
            StopCoroutine("SpawnPowerup");
            WaveManager();
            _previousWavePower = _spawnWeight;
        }

    }

    private void WaveManager()
    {
        do
        {
            _spawnWeight = SpawnWeight();
        }
        while (_previousWavePower >= _spawnWeight);
        _spawnWeight = SpawnWeight();
        while (_spawnWeight > 0)
        {
            SpawnSelector(out int _selectedEnemy, out float _enemyWeight);
            _spawnWeight -= _enemyWeight;
            if (_selectedEnemy == 0)
            {
                _number0ToSpawn += 1;
            }
            if (_selectedEnemy == 1)
            {
                _number1ToSpawn += 1;
            }
        }
        _enemiesSpawned = _number0ToSpawn + _number1ToSpawn;
        _spawnRateModifier = _difficultyCurve.Evaluate(_currentWave) * 0.2f;
        _balancedMLSpawnTime = _number0ToSpawn * (4 - _spawnRateModifier) / _number1ToSpawn;
        StartCoroutine(SpawnEnemy(_number0ToSpawn));
        StartCoroutine(SpawnMinelayer(_number1ToSpawn));
        StartCoroutine("SpawnPowerup");
    }

    public float SpawnWeight()
    {
        float _waveDifficulty = 0;
        float _waveWeightModifier = 200;
        float _weightToSpawn = 0;

        _waveDifficulty = _difficultyCurve.Evaluate(_currentWave);
        _weightToSpawn = _waveDifficulty * _waveWeightModifier;
        return _weightToSpawn;
    } 

    private void SpawnSelector(out int _selectedEnemy, out float _enemyWeight)
    {
        float _weightedsum = 0;
        float _randomweight = 0;
        _selectedEnemy = 0;
        _enemyWeight = 0;

        foreach (KeyValuePair<int, float> _enemyValues in _enemySpawn)
        {
            _weightedsum += _enemyValues.Value;
        }
        do
        {
            _randomweight = Random.Range(0, _weightedsum);
        }
        while (_randomweight == _weightedsum);
        foreach (KeyValuePair<int, float> _enemyValues in _enemySpawn)
        {
            if (_randomweight < _enemyValues.Value)
            {
                _selectedEnemy =_enemyValues.Key;
                _enemyWeight = _enemyValues.Value;
            }
            else _randomweight -= _enemyValues.Value;
        }
    }

    IEnumerator SpawnEnemy(int _spawnNumber)
    {
        while (_playeralive == true && _spawnNumber > 0)
        {
            yield return new WaitForSeconds(3.5f - _spawnRateModifier);
            if (_playeralive == true)
            {
                GameObject _newEnemy = Instantiate(_enemyShips[0], new Vector3(Random.Range(-19.5f, 19.5f), 11.5f, 0.0f), Quaternion.identity);
                _newEnemy.transform.parent = _enemyContainer.transform;
            }
            _spawnNumber--;
        }
    }

    IEnumerator SpawnMinelayer(int _spawnNumber)
    {
        int _side;
        while (_playeralive == true && _spawnNumber > 0)
        {
            if (Random.value < 0.5f)
            {
                _side = 1;
            }
            else _side = -1;
            yield return new WaitForSeconds( _balancedMLSpawnTime);
            if (_playeralive == true)
            {
                GameObject _newMinelayer = Instantiate(_enemyShips[1], new Vector3(19.4f * _side, 7.75f, 0), Quaternion.identity);
                _newMinelayer.transform.parent = _enemyContainer.transform;
            }
            _spawnNumber--;
        }
    }

    IEnumerator SpawnPowerup()
    {
        while (_playeralive == true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f) - _spawnRateModifier);
            if (_playeralive == true)
            {
                Instantiate(_poweruptype[PowerUpSelector()], new Vector3(Random.Range(-19.5f, 19.5f), 11.5f, 0.0f), Quaternion.identity);
            }
        }
    }
    public void OnPlayerDeath()
    {
        _playeralive = false;
        StartCoroutine(_uiManager.GameOverDisplayCo());
    }

    private int PowerUpSelector()
    {
        float _weightedsum = 0;
        float _randomweight = 0;

        foreach (KeyValuePair<int, float> _powerupvalues in _powerups)
        {
            _weightedsum += _powerupvalues.Value;
        }
        do
        {
            _randomweight = Random.Range(0, _weightedsum);
        }
        while (_randomweight == _weightedsum);
        foreach (KeyValuePair<int, float> _powerupvalues in _powerups)
        {
            if (_randomweight < _powerupvalues.Value)
            {
                return _powerupvalues.Key;
            }
            else _randomweight -= _powerupvalues.Value;
        };
        Debug.LogError("PowerUpSelector is out of range");  //these 2 lines at the end should never happen
        return 0;  //this is only here to suppress an error
    }

    public void KillTracker()
    {
        _killsMade++;
        if (_killsMade == _enemiesSpawned)
        {
            _waveComplete = true;
            _killsMade = 0;
        }
        

    }
}

