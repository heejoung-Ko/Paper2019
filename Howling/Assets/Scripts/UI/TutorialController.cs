using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class TutorialController : MonoBehaviour
    {
        private GameObject currentTutorial = null;
        private float startTime = 1f;
        private float readyTime = 2f;
        private float deleteTime = 0.5f;
        private float delayDeleteTime = 3f;

        [HideInInspector] public int currentShow = 0;
        private int maxShow = 6;
        [HideInInspector] public bool isPlayerMove = false;
        [HideInInspector] public bool isPlayerRun = false;
        [HideInInspector] public bool isPlayerRotation = false;
        [HideInInspector] public bool isPlayerGetItem = false;

        private void Start()
        {
            StartCoroutine(TutorialLoop(startTime));
        }

        private IEnumerator TutorialLoop(float delayTime)
        {
            yield return StartCoroutine(DelayTime(delayTime));
            //currentTutorial = transform.GetChild(currentShow).gameObject;
            //currentTutorial.SetActive(true);
            currentShow++;
            switch (currentShow)
            {
                case 1:
                    yield return ShowMove();
                    break;
                case 2:
                    yield return ShowRun();
                    break;
                case 3:
                    yield return ShowRotation();
                    break;
                case 4:
                    yield return ShowGetItem();
                    break;
            }
            if (currentShow < maxShow)
                StartCoroutine(TutorialLoop(readyTime));
            else
                Debug.Log("튜토리얼 끝!");
        }

        private IEnumerator ShowMove()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerMove)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator ShowRun()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerRun)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator ShowRotation()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerRotation)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(delayDeleteTime));
        }

        private IEnumerator ShowGetItem()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerGetItem)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator DelayTime(float deleyTime)
        {
            yield return new WaitForSeconds(deleyTime);
            if (currentTutorial != null)
            {
                currentTutorial.SetActive(false);
                currentTutorial = null;
            }
        }
    }
}