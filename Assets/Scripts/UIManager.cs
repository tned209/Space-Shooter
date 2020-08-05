using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Text _scoreText = default;
    [SerializeField]
    private Image _livesimage = default;
    [SerializeField]
    private Sprite[] _livesprites = default;
    [SerializeField]
    private Text _gameover = default;
    [SerializeField]
    private Text _restart = default;
    [SerializeField]
    private Text _ammoText = default;
    [SerializeField]
    private GameObject _boostHealthBar = default;
    private GameObject[] _ammoIcons = default;
    [SerializeField]
    private Text _waveBanner = default;


    // Start is called before the first frame update
    void Start()
    {
        _gameover.gameObject.SetActive(false);
        _restart.gameObject.SetActive(false);
        _ammoIcons = GameObject.FindGameObjectsWithTag("AmmoCounter");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUIScore(int _score)
    {
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void BoostMeter(float _turbohealth)
    {
        _boostHealthBar.transform.localScale = new Vector3(_turbohealth / 3f, 1f, 1f);
    }

    public void UpdateLives(int _currentlives)
    {
        _livesimage.sprite = _livesprites[_currentlives];
    }

    public void UpdateAmmo(int _ammocount, bool _shotpowerupactive, float _shotactivetime)  
    {
        if (_shotpowerupactive == false)
        {
            _ammoText.text = "Ammo Remaining: " + _ammocount.ToString() + "/15";
        }
        else _ammoText.text = ("Free Fire for " + _shotactivetime.ToString() + " seconds");

        for (int i = 0; i < _ammoIcons.Length; i++)
        {
            if (i <= _ammocount - 1)
            {
                _ammoIcons[i].gameObject.SetActive(true);
            }
            if (i > _ammocount - 1)
            {
                _ammoIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator WaveAnnounce(int _currentWave)
    {
        _waveBanner.text = ("Wave " + _currentWave);
        _waveBanner.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _waveBanner.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveBanner.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _waveBanner.gameObject.SetActive(false);
    }

    public IEnumerator GameOverDisplayCo()
    {
        _restart.gameObject.SetActive(true);
        while (true)
        {
            _gameover.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            _gameover.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
        }
        
    }
}
