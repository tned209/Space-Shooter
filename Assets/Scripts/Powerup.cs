using UnityEngine;

public class Powerup : MonoBehaviour
{
    private float _powerupspeed = 4.0f;
    private int _pointvalue = 500;
    [SerializeField]
    private int _powerupID = default;  //0 = tripleshot  1 = speedboost  2 = shield  3 = reload  4 = health  5 = waveshot
    private GameObject PowerUpSoundRef;
    private AudioSource _powerupsfx;
    private AudioClip _powerupclip;
    private GameObject _reloadRef = default;
    private AudioSource _reloadsource = default;
    private AudioClip _reloadclip = default;
    private GameObject _powerdownRef = default;
    private AudioSource _powerdownSource = default;

    // Start is called before the first frame update
    void Start()
    {
        PowerUpSoundRef = GameObject.Find("power_up_sound");
        _powerupsfx = PowerUpSoundRef.GetComponent<AudioSource>();
        _powerupclip = _powerupsfx.clip;
        _reloadRef = GameObject.Find("reload");
        _reloadsource = _reloadRef.GetComponent<AudioSource>();
        _reloadclip = _reloadsource.clip;
        _powerdownRef = GameObject.Find("power_down_sound");
        _powerdownSource = _powerdownRef.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= -9.5f)
        {
            transform.Translate(new Vector3(0, -1 * _powerupspeed * Time.deltaTime, 0));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player _player = other.transform.GetComponent<Player>();
            if (_player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        _player.TripleShotActive();
                        break;
                    case 1:
                        _player.SpeedActive();
                        break;
                    case 2:
                        _player.ShieldActive();
                        break;
                    case 3:
                        _player.Reload();
                        break;
                    case 4:
                        _player.HealthManagement(true);
                        break;
                    case 5:
                        _player.WaveShotActive();
                        break;
                    case 6:
                        _player.PowerDown();
                        break;
                    default:
                        Debug.LogError("Powerup activation switch defaulted");
                        break;
                }
            }
            
            if (_powerupID !=4 && _powerupID != 6)
            {
                _powerupsfx.PlayOneShot(_powerupclip, 1.0f);
            }
            if (_powerupID == 3)
            {
                _reloadsource.PlayOneShot(_reloadclip, 1.0f);
            }
            if (_powerupID == 6)
            {
                _powerdownSource.PlayOneShot(_powerdownSource.clip, 1.0f);
            }

            Destroy(this.gameObject);
            if (_powerupID != 6)
            {
                _player.TrackScore(_pointvalue);
            }
        }
    }
}
