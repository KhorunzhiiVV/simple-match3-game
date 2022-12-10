using System;
using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class ClearableGem : MonoBehaviour
{
    [SerializeField] private AnimationClip _clearAnimation;

    
    private bool _isBeingCleared = false;
    public bool IsBeingCleared
    { get { return _isBeingCleared; } }

    protected Gem _gem;
    private Player _player;

    private void Awake()
    {
        _gem = GetComponent<Gem>();
        _player = FindObjectOfType<Player>();
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

            //_player.Attack(_gem.ColorComponent.Color);
            Destroy(gameObject);
        }
    }


}
