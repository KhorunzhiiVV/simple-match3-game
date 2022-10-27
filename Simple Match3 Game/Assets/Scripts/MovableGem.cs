using System;
using System.Collections;
using UnityEngine;

public class MovableGem : MonoBehaviour
{
    private Gem gem;
    private IEnumerator moveCoroutine;


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

    public void Move (int newX, int newY, float animationTime)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, animationTime);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float animationTime)
    {
        gem.X = newX;
        gem.Y = newY;
        
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(newX, newY, 0);

        for (float t = 0; t <= 1 * animationTime; t += Time.deltaTime)
        {
            gem.transform.position = Vector3.Lerp(startPosition, endPosition, t / animationTime);
            yield return null;
        }

        gem.transform.position = endPosition;
    }
}
