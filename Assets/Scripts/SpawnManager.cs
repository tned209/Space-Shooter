using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab = default;
    [SerializeField]
    private GameObject _enemyContainer = default;
    private bool _playeralive = true;
    [SerializeField]
    private GameObject[] _poweruptype = default;

    // Start is called before the first frame update
    void Start()
    {
       
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
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    IEnumerator SpawnEnemy()
    {
        while (_playeralive == true)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
            if (_playeralive == true)
            {
                GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-9.5f, 9.5f), 5.75f, 0.0f), Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }
        }
    }

    IEnumerator SpawnPowerup()
    {
        

        while (_playeralive == true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
            if (_playeralive == true)
            {
                Instantiate(_poweruptype[PowerUpSelector()], new Vector3(Random.Range(-9.5f, 9.5f), 5.75f, 0.0f), Quaternion.identity);
            }
        }
    }
    public void OnPlayerDeath()
    {
        _playeralive = false;
        UIManager uimanager = GameObject.FindObjectOfType<UIManager>();
        StartCoroutine(uimanager.GameOverDisplayCo());
    }

    private int PowerUpSelector()
    {
        float _weightedsum = 0;
        float _randomweight = 0;

        Dictionary<int, float> _powerups = new Dictionary<int, float>() //_powerupID from powerupscript, spawn weight
        {
            { 0, 5f },
            { 1, 5f },
            { 2, 5f },
            { 3, 10f },
            { 4, 3f },
            { 5, 2f }
        };

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
}

