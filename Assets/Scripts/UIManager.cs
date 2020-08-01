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

    // Start is called before the first frame update
    void Start()
    {
        _gameover.gameObject.SetActive(false);
        _restart.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUIScore(int _score)
    {
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void UpdateLives(int _currentlives)
    {
        _livesimage.sprite = _livesprites[_currentlives];
    }

    public void UpdateAmmo(int _ammocount, bool _shotpowerupactive, float _shotactivetime, bool _synergy)  
    {
        if (_synergy == true)
        {
            _ammoText.text = (" SYNERGY Free Fire for " + _shotactivetime.ToString() + " seconds");
            return;
        }
        if (_shotpowerupactive == false)
        {
            _ammoText.text = "Ammo Remaining: " + _ammocount.ToString();
        }
        else _ammoText.text = ("Free Fire for " + _shotactivetime.ToString() + " seconds");
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
