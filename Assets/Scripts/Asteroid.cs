using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Animator _animator;
    private Player _player;
    private SpawnManager _spawnmanager;
    [SerializeField]
    private GameObject _asteroidExplosionPrefab = default;
    private GameObject ExplosionRef;
    private AudioSource _asteroidexplosionsfx;
    private AudioClip _rockboom;


    // Start is called before the first frame update
    void Start()
    {
        _animator = this.gameObject.GetComponent<Animator>();
        _player = GameObject.FindObjectOfType<Player>();
        _spawnmanager = GameObject.FindObjectOfType<SpawnManager>();
        ExplosionRef = GameObject.Find("explosion_sound");
        _asteroidexplosionsfx = ExplosionRef.GetComponent<AudioSource>();
        _rockboom = _asteroidexplosionsfx.clip;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 30 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _asteroidexplosionsfx.PlayOneShot(_rockboom, 1.0f);
            GameObject _boom = Instantiate(_asteroidExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.3f);
            _player.Damage();
            Destroy(_boom, 2.35f);
            _spawnmanager.StartSpawning();
        }
        if (other.tag == "Laser")
        {
            _asteroidexplosionsfx.PlayOneShot(_rockboom, 1.0f);
            GameObject _boom = Instantiate(_asteroidExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.3f);
            Destroy(other.gameObject);
            Destroy(_boom, 2.35f);
            _spawnmanager.StartSpawning();
        }
    }
}
