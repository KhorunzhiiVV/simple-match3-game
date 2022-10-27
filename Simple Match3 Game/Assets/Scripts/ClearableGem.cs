using System.Collections;
using UnityEngine;

public class ClearableGem : MonoBehaviour
{
    [SerializeField] private AnimationClip _clearAnimation;

    private bool _isBeingCleared = false;
    public bool IsBeingCleared
    { get { return _isBeingCleared; } }

    protected Gem gem;

    private void Awake()
    {
            gem = GetComponent<Gem>();
    }



    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public virtual void Clear()
    {
        _isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine ()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(_clearAnimation.name);

            yield return new WaitForSeconds(_clearAnimation.length);

            Destroy(gameObject);
        }
    }


}
