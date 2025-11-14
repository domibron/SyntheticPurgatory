using System;
using System.Collections;
using UnityEngine;

// TODO: Shit name, rename later
/// <summary>
/// manages the kill dozers gates and attacks one at a time.
/// </summary>
public class OshaViolationManager : MonoBehaviour
{
    [SerializeField]
    Dozer[] dozers;

    private bool inJob = false;

    [SerializeField]
    private float coolDown = 3f;


    private float currentCoolDown = 0f;

    void Awake()
    {
        foreach (Dozer dozer in dozers)
        {
            dozer.OnJobCompleted += OnJobCompleted;
        }
    }

    void Update()
    {
        if (currentCoolDown > 0) currentCoolDown -= Time.deltaTime;

        StartAnAttack(); // ! DEBUG CODE
    }

    public bool StartAnAttack()
    {
        if (inJob || currentCoolDown > 0) return false;

        StartCoroutine(StartDozerAttack());
        return true;
    }

    private IEnumerator StartDozerAttack()
    {
        inJob = true;

        Dozer dozer = dozers[UnityEngine.Random.Range(0, dozers.Length)];

        while (!dozer.TryToStartAttack())
        {
            dozer = dozers[UnityEngine.Random.Range(0, dozers.Length)];
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnJobCompleted()
    {
        inJob = false;
        currentCoolDown = coolDown;
    }
}
