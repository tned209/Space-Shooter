using System.Collections;
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
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
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
                Instantiate(_poweruptype[Random.Range(0, 5)], new Vector3(Random.Range(-9.5f, 9.5f), 5.75f, 0.0f), Quaternion.identity);
            }
        }
    }
    public void OnPlayerDeath()
    {
        _playeralive = false;
        UIManager uimanager = GameObject.FindObjectOfType<UIManager>();
        StartCoroutine(uimanager.GameOverDisplayCo());
    }
}

