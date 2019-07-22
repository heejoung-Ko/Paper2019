﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class TutorialController : MonoBehaviour
    {
        private GameObject currentTutorial = null;
        private float startTime = 4f;
        private float readyTime = 2f;
        private float deleteTime = 1f;
        private float delayDeleteTime = 3f;

        public int currentShow = 0;
        private int maxShow = 6;
        [HideInInspector] public bool isPlayerMove = false;
        [HideInInspector] public bool isPlayerRun = false;
        [HideInInspector] public bool isPlayerRotation = false;
        [HideInInspector] public bool isPlayerGetItem = false;
        [HideInInspector] public bool isPlayerAttack = false;
        [HideInInspector] public bool isPlayerUseItem = false;

        private void Start()
        {
            // StartCoroutine(TutorialLoop(startTime));
        }

        private IEnumerator TutorialLoop(float delayTime)
        {
            yield return StartCoroutine(DelayTime(delayTime));
            currentShow++;
            Debug.Log("currentShow: " + currentShow);
            currentTutorial = transform.GetChild(currentShow).gameObject;
            currentTutorial.SetActive(true);
            switch (currentShow)
            {
                case 1: yield return ShowRotation(); break;
                case 2: yield return ShowMove(); break;
                case 3: yield return ShowRun(); break;
                case 4: yield return ShowGetItem(); break;
                case 5: yield return ShowAttack(); break;
                case 6: yield return ShowUseItem(); break;
            }
            if (currentShow < maxShow)
            {
                StartCoroutine(TutorialLoop(readyTime));
            }
            else
                Debug.Log("튜토리얼 끝!");
        }

        private IEnumerator ShowRotation()
        {
            while (!isPlayerRotation)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(delayDeleteTime));
        }

        private IEnumerator ShowMove()
        {
            while (!isPlayerMove)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator ShowRun()
        {
            while (!isPlayerRun)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator ShowGetItem()
        {
            while (!isPlayerGetItem)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(deleteTime));
        }

        private IEnumerator ShowAttack()
        {
            while (!isPlayerAttack)
            {
                yield return null;
            }
            yield return StartCoroutine(DelayTime(delayDeleteTime));
        }

        private IEnumerator ShowUseItem()
        {
            while (!isPlayerUseItem)
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