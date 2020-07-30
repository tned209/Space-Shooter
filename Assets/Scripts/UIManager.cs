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
