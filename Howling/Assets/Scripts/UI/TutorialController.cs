using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class TutorialController : MonoBehaviour
    {
        private GameObject currentTutorial = null;
        private float startTime = 2f;
        private float deleteTime = 3f;

        public int currentShow = 0;
        private int maxShow = 6;
        public bool isPlayerMove = false;
        public bool isPlayerRotation = false;

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
                break;
                case 3:
                yield return ShowRotation();
                break;
            }
            if (currentShow < maxShow)
                StartCoroutine(TutorialLoop(startTime));
            else
                Debug.Log("튜토리얼 끝!");
            Debug.Log("currentShow - " + currentShow);
        }

        private IEnumerator ShowMove()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerMove)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(0f));
        }

        private IEnumerator ShowRotation()
        {
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            while (!isPlayerRotation)
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