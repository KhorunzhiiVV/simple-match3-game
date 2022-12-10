using System;
using System.Collections;
using UnityEngine;

public class MovableGem : MonoBehaviour
{
    private Gem _gem;
    private IEnumerator _moveCoroutine;


    private void Awake()
    {
        _gem = GetComponent<Gem>();
    }

    public void Move (int newX, int newY, float animationTime)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(newX, newY, animationTime);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float animationTime)
    {
        _gem.X = newX;
        _gem.Y = newY;
        
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(newX, newY, 0);

        for (float t = 0; t <= 1 * animationTime; t += Time.deltaTime)
        {
            _gem.transform.position = Vector3.Lerp(startPosition, endPosition, t / animationTime);
            yield return null;
        }

        _gem.transform.position = endPosition;
    }
}
