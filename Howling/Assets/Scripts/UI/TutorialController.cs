using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class TutorialController : MonoBehaviour
    {
        private float startTime = 1f;
        private float moveTime = 1f;
        private GameObject currentTutorial = null;

        private void Start()
        {
            StartCoroutine(StartTutorial());
        }

        private IEnumerator StartTutorial()
        {
            yield return StartCoroutine(DelayTime(startTime));
            yield return ShowMove();
        }

        private IEnumerator ShowMove()
        {
            currentTutorial = transform.FindChild("Tutorial_Move").gameObject;
            currentTutorial.SetActive(true);
            yield return StartCoroutine(DelayTime(moveTime));
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