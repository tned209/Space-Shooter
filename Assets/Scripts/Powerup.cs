using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupspeed = 3.0f;
    private int _pointvalue = 500;
    [SerializeField]
    private int _powerupID;  //0 = tripleshot  1 = speedboost  2 = shield
    private GameObject PowerUpSoundRef;
    private AudioSource _powerupsfx;
    private AudioClip _powerupclip;
    
    // Start is called before the first frame update
    void Start()
    {
        PowerUpSoundRef = GameObject.Find("power_up_sound");
        _powerupsfx = PowerUpSoundRef.GetComponent<AudioSource>();
        _powerupclip = _powerupsfx.clip;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= -5.75f)
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
                    default:
                        Debug.LogError("Powerup activation switch defaulted");
                        break;
                }
            }
            _powerupsfx.PlayOneShot(_powerupclip, 1.0f);
            Destroy(this.gameObject);
            _player.TrackScore(_pointvalue);
        }
    }
}
